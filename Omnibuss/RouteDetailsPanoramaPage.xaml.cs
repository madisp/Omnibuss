using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Shell;
using System.Device.Location;
using MarkerClustering;
using System.ComponentModel; //---for Debug.WriteLine()---

namespace Omnibuss
{
    public partial class RouteDetailsPanoramaPage : PhoneApplicationPage
    {

        List<Pushpin> pins;
        GeoCoordinateWatcher watcher;
        Location myLocation;

        public RouteDetailsPanoramaPage()
        {
            InitializeComponent();

            pins = new List<Pushpin>();

            var clusterer = new PushpinClusterer(map1, pins, this.Resources["ClusterTemplate"] as DataTemplate);

            SystemTray.SetIsVisible(this, true);

            if (watcher == null)
            {
                //---get the highest accuracy---
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
                {
                    //---the minimum distance (in meters) to travel before the next 
                    // location update---
                    MovementThreshold = 10
                };

                //---event to fire when a new position is obtained---
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

                //---event to fire when there is a status change in the location 
                // service API---
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.Start();

                map1.Center = new GeoCoordinate(58.383333, 26.716667);
                map1.ZoomLevel = 15;
                map1.Mode = new MyMapMode();
            }
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    Debug.WriteLine("disabled");
                    break;
                case GeoPositionStatus.Initializing:
                    Debug.WriteLine("initializing");
                    break;
                case GeoPositionStatus.NoData:
                    Debug.WriteLine("nodata");
                    break;
                case GeoPositionStatus.Ready:
                    Debug.WriteLine("ready");
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Debug.WriteLine("({0},{1})", e.Position.Location.Latitude, e.Position.Location.Longitude);
            if (!e.Position.Location.IsUnknown)
            {
                myLocation = e.Position.Location;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String stopIdStr, routeIdStr;
            if (!NavigationContext.QueryString.TryGetValue("stopId", out stopIdStr))
            {
                Debug.WriteLine("No stop ID provided!");
                NavigationService.GoBack();
                return;
            }

            if (!NavigationContext.QueryString.TryGetValue("routeId", out routeIdStr))
            {
                Debug.WriteLine("No route ID provided!");
                NavigationService.GoBack();
                return;
            }

            UInt32 stopId = UInt32.Parse(stopIdStr);
            Debug.WriteLine("Stop id: " + stopId);
            UInt32 routeId = UInt32.Parse(routeIdStr);
            Debug.WriteLine("Route id: " + routeId);

            OmnibussModel model = new OmnibussModel();

            Stop stop = model.GetStop(stopId);
            Route route = model.GetRoute(routeId);

            List<Stop_time> times = new List<Stop_time>();
            List<List<Stop>> stopsList = new List<List<Stop>>();

            ProgressIndicator progress = new ProgressIndicator();
            progress.IsVisible = true;
            progress.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, progress);

            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(
                (sender, args) =>
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Trip trip = model.GetMaxTripByRoute(route, i);
                        if (trip != null)
                        {
                            Debug.WriteLine("TripID: " + trip.Trip_id);
                            List<Stop> stops = model.GetStopsByTrip(trip);
                            stopsList.Add(stops);
                        }
                    }

                    times.AddRange(model.GetTimesByRouteAndStop(route, stop));
                    
                }
            );
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                (sender, args) =>
                {
                    if (times.Count == 0)
                    {
                        NextTime.Text = "Kahjuks see liin täna rohkem ei sõida.";
                    }
                    else
                    {
                        DateTime now = DateTime.Now;
                        int diff = (int)times[0].Departure_time - (now.Hour * 10000 + now.Minute * 100 + now.Second);

                        double _diff = 0.0;
                        _diff += diff % 100;
                        diff /= 100;
                        _diff += (diff % 100) * 60;
                        diff /= 100;
                        _diff += (diff % 100) * 60;

                        double timeInHours = _diff / 3600.0;
                        // TEMP END

                        NextTime.Text = "Buss väljub järgnevatel aegadel:";
                        Location stopLocation = new Location();
                        stopLocation.Latitude = (double)stop.Latitude;
                        stopLocation.Longitude = (double)stop.Longitude;
                        double distanceKm = calculateDistance(myLocation, stopLocation);
                        string warningText = "";
                        if (distanceKm > (10 * timeInHours))
                        {
                            warningText = " (You'll not make it)";
                        }
                        else if (distanceKm < (10 * timeInHours) && distanceKm > (5 * timeInHours))
                        {
                            warningText = " (Run, you can still make it!)";
                        }
                        NextTime.Text = "Buss väljub järgnevatel aegadel" + warningText + ":";

                    }

                    var timesList = new List<String>();
                    
                    foreach (var time in times)
                    {
                        String timeString = time.Departure_time.ToString();
                        String hours = timeString.Substring(0, timeString.Length > 5 ? 2 : 1);
                        String minutes = timeString.Substring(timeString.Length > 5 ? 2 : 1, 2);
                        timesList.Add(String.Format("{0,2:d2}:{1,2:d2}", hours, minutes));
                    }

                    schedule.ItemsSource = timesList;
                    foreach (var stops in stopsList)
                    {
                        GetRoute(stops);
                        foreach (var _stop in stops)
                        {
                            Pushpin pin = addLocationPin(_stop.Latitude, _stop.Longitude, _stop.Name);
                            int id = _stop.Id;

                            pin.MouseLeftButtonUp += new MouseButtonEventHandler(
                                (sender1, e1) =>
                                {
                                    NavigationService.Navigate(new Uri("/StopDetailsPanoramaPage.xaml?stopId=" + id, UriKind.Relative));
                                });
                        }
                    }
                    progress.IsVisible = false;
                }
            );
            bgWorker.RunWorkerAsync();


            Panorama.Title = route;

            var panoramaItem = Panorama.Items[0] as PanoramaItem;
            if (panoramaItem != null)
            {
                panoramaItem.Header = stop.Name;
            }
        }

        private double calculateDistance(Location l1, Location l2)
        {
            double e = (3.1415926538 * l1.Latitude / 180);
            double f = (3.1415926538 * l1.Longitude / 180);
            double g = (3.1415926538 * l2.Latitude / 180);
            double h = (3.1415926538 * l2.Longitude / 180);
            double i = (Math.Cos(e) * Math.Cos(g) * Math.Cos(f) * Math.Cos(h) + Math.Cos(e) * Math.Sin(f) * Math.Cos(g) * Math.Sin(h) + Math.Sin(e) * Math.Sin(g));
            double j = (Math.Acos(i));
            double k = (6371 * j);

            return k;
        }


        Pushpin addLocationPin(double? latitude, double? longitude, object content)
        {
            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate((double)latitude, (double)longitude);
            pin.Content = content;
            pins.Add(pin);
            return pin;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void GetRoute(List<Stop> stops)
        {
            // Create the service variable and set the callback method using the CalculateRouteCompleted property.
            RouteService.RouteServiceClient routeService = new RouteService.RouteServiceClient("BasicHttpBinding_IRouteService");
            routeService.CalculateRouteCompleted += new EventHandler<RouteService.CalculateRouteCompletedEventArgs>(routeService_CalculateRouteCompleted);

            // Set the token.
            RouteService.RouteRequest routeRequest = new RouteService.RouteRequest();
            routeRequest.Credentials = new Credentials();
            routeRequest.Credentials.ApplicationId = ((ApplicationIdCredentialsProvider)map1.CredentialsProvider).ApplicationId;

            // Return the route points so the route can be drawn.
            routeRequest.Options = new RouteService.RouteOptions();
            routeRequest.Options.RoutePathType = RouteService.RoutePathType.Points;

            routeRequest.Waypoints = new System.Collections.ObjectModel.ObservableCollection<RouteService.Waypoint>();
            int max = stops.Count() > 25 ? 25 : stops.Count();
            for (int i = 0; i < max; i++)
            {
                Stop stop = stops.ElementAt(i);
                RouteService.Waypoint srcWaypoint = new RouteService.Waypoint();
                srcWaypoint.Location = new GeoCoordinate((double)stop.Latitude, (double)stop.Longitude);
                routeRequest.Waypoints.Add(srcWaypoint);
            }



            // Make the CalculateRoute asnychronous request.
            routeService.CalculateRouteAsync(routeRequest);

        }

        private void routeService_CalculateRouteCompleted(object sender, RouteService.CalculateRouteCompletedEventArgs e)
        {

            // If the route calculate was a success and contains a route, then draw the route on the map.
            if ((e.Result.ResponseSummary.StatusCode == RouteService.ResponseStatusCode.Success) & (e.Result.Result.Legs.Count != 0))
            {
                // Set properties of the route line you want to draw.
                Color routeColor = Colors.Blue;
                SolidColorBrush routeBrush = new SolidColorBrush(routeColor);
                MapPolyline routeLine = new MapPolyline();
                routeLine.Locations = new LocationCollection();
                routeLine.Stroke = routeBrush;
                routeLine.Opacity = 0.65;
                routeLine.StrokeThickness = 5.0;

                // Retrieve the route points that define the shape of the route.
                foreach (Location p in e.Result.Result.RoutePath.Points)
                {
                    Location location = new Location();
                    location.Latitude = p.Latitude;
                    location.Longitude = p.Longitude;
                    routeLine.Locations.Add(location);
                }

                // Add a map layer in which to draw the route.
                MapLayer myRouteLayer = new MapLayer();
                map1.Children.Add(myRouteLayer);

                // Add the route line to the new layer.
                myRouteLayer.Children.Add(routeLine);


            }
        }
    }
}