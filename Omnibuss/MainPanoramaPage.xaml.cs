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
using System.Diagnostics; //---for Debug.WriteLine()---
using System.Xml.Linq;
using Microsoft.Phone.Controls.Maps.Platform;
using System.Globalization;
using MarkerClustering;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using Newtonsoft.Json;
using Microsoft.Phone.Reactive;

namespace Omnibuss
{
    public partial class MainPanoramaPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher watcher;

        List<Pushpin> pins;

        // Constructor
        public MainPanoramaPage()
        {
            InitializeComponent();

            pins = new List<Pushpin>();

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
                var clusterer = new PushpinClusterer(map1, pins, this.Resources["ClusterTemplate"] as DataTemplate);

                // get list of stops
                OmnibussModel model = new OmnibussModel();
                List<Stop> stops = model.GetStops();
                Debug.WriteLine("Stops count: " + stops.Count);

                foreach (Stop stop in stops)
                {
                    Pushpin pin = addLocationPin(stop.Latitude, stop.Longitude, stop.Name);
                    int id = stop.Id;

                    pin.MouseLeftButtonUp += new MouseButtonEventHandler(
                        (object sender, MouseButtonEventArgs e) =>
                        {
                            NavigationService.Navigate(new Uri("/StopDetailsPanoramaPage.xaml?stopId=" + id, UriKind.Relative));
                        });
                }
            }

            LoadTickets();
        }

        private void LoadTickets()
        {
            var w = new WebClient();
            Observable.FromEvent<DownloadStringCompletedEventArgs>(w, "DownloadStringCompleted").Subscribe(r =>
              {
                  Debug.WriteLine("JOTSON: " + r.EventArgs.Result);
                  try
                  {
                      var deserialized = JsonConvert.DeserializeObject<List<Ticket>>(r.EventArgs.Result);
                      tickets.ItemsSource = deserialized;
                  }
                  catch (Exception)
                  {
                      Debug.WriteLine("Okou..");
                  }
              });
            w.DownloadStringAsync(new Uri("http://office.mobi.ee/~sigmar/pilet.txt"));
            //w.DownloadStringAsync(new Uri("http://office.mobi.ee/~sigmar/ticket.php?id_code=A1668485"));
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
            // Debug.WriteLine("({0},{1})", e.Position.Location.Latitude, e.Position.Location.Longitude);

            // map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
            // Pushpin pin = addLocationPin(e.Position.Location.Latitude, e.Position.Location.Longitude, "My Location");
            // pin.MouseLeftButtonUp += new MouseButtonEventHandler(myLocation_Click);
        }

        //void myLocation_Click(object sender, MouseButtonEventArgs e)
        //{
        //    Debug.WriteLine("Siin ma olengi :)");
        //    Pushpin pin = (Pushpin)sender;
        //    pin.Content = "Juuuuhuuuu!";
       // }

        Pushpin addLocationPin(double? latitude, double? longitude, object content)
        {
            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate((double)latitude, (double)longitude);
            pin.Content = content;
            pins.Add(pin);
            return pin;
        }

    }

    public class MyMapMode : Microsoft.Phone.Controls.Maps.RoadMode
    {
        public Range<double> MapZoomRange = new Range<double>(10.0, 100.0);
        protected override Range<double> GetZoomRange(GeoCoordinate center)
        {
            return MapZoomRange;
        }
    }
}