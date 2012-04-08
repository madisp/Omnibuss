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
using System.Device.Location;
using MarkerClustering; //---for Debug.WriteLine()---

namespace Omnibuss
{
    public partial class RouteDetailsPanoramaPage : PhoneApplicationPage
    {

        List<Pushpin> pins;

        public RouteDetailsPanoramaPage()
        {
            InitializeComponent();

            pins = new List<Pushpin>();

            var clusterer = new PushpinClusterer(map1, pins, this.Resources["ClusterTemplate"] as DataTemplate);

            map1.Center = new GeoCoordinate(58.383333, 26.716667);
            map1.ZoomLevel = 15;
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

            for (int i = 0; i < 2; i++)
            {
                Trip trip = model.GetMaxTripByRoute(route, i);
                Debug.WriteLine("TripID: " + trip.Trip_id);
                List<Stop> stops = model.GetStopsByTrip(trip);
                GetRoute(stops);
                foreach (Stop _stop in stops)
                {
                    Pushpin pin = addLocationPin(_stop.Latitude, _stop.Longitude, _stop.Name);
                }
            }
            Panorama.Title = route;

            List<String> timesList = new List<String>();
            //timesList.Add("12:00");
            //timesList.Add("12:25");
            //timesList.Add("12:50");
            //timesList.Add("13:15");
            //timesList.Add("13:40");
            //timesList.Add("14:05");
            //timesList.Add("14:30");
            //timesList.Add("14:55");
            //timesList.Add("15:20");
            List<Stop_time> times = model.GetTimesByRouteAndStop(route, stop);
            foreach (var time in times) {
               timesList.Add(time.Departure_time.ToString());
            }
            schedule.ItemsSource = timesList;
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
            foreach (Stop stop in stops)
            {
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