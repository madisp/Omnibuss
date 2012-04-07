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
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using System.Diagnostics; //---for Debug.WriteLine()---
using System.Xml.Linq;
using Microsoft.Phone.Controls.Maps;

namespace Omnibuss
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher watcher;
        StopDTO lastStop;

        String requestString = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=lynnwood&wp.1=seattle&avoid=minimizeTolls&key=Aj2gDlArPAqNxkeyI11APMNS_g_1RYAj9yJgEXxYcXQB2nU7BWTJQkACS8js5_Kr";
        WebClient wc;

        // Constructor
        public MainPage()
        {


            InitializeComponent();

            // Create the WebClient and associate a handler with the OpenReadCompleted event.
            wc = new WebClient();
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
            CallToWebService();

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

                map1.Center = new GeoCoordinate(47.676289396624654, -122.12096571922302);
                map1.ZoomLevel = 1;

                // get list of stops
                OmnibussModel model = new OmnibussModel();
                List<Stop> stops = model.getStops();
                Debug.WriteLine("Stops count: " + stops.Count);

                foreach (Stop stop in stops)
                {
                    Pushpin pin = addLocationPin(stop.Latitude, stop.Longitude, stop.Name);
                    pin.MouseLeftButtonUp += new MouseButtonEventHandler(delegate(object sender, MouseButtonEventArgs e)
                    {
                        NavigationService.Navigate(new Uri("/StopDetails.xaml?stopId=" + stop.Id, UriKind.Relative));
                    });
                }
            }
        }

        private void CallToWebService()
        {
            // Call the OpenReadAsyc to make a get request, passing the url with the selected search string.
            wc.OpenReadAsync(new Uri(requestString));
        }
        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            XElement resultXml;
            // You should always check to see if an error occurred. In this case, the application
            // simply returns.
            if (e.Error != null)
            {
                   return;
            }
            else
            {
                XNamespace web = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/web";
                try
                {
                    resultXml = XElement.Load(e.Result);
                    Debug.WriteLine("Heihoo: " + resultXml.FirstNode.ToString());
                }
                catch (System.Xml.XmlException ex)
                {
                    
                
                }
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
            map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
            Pushpin pin = addLocationPin(e.Position.Location.Latitude, e.Position.Location.Longitude, "My Location");
            pin.MouseLeftButtonUp += new MouseButtonEventHandler(myLocation_Click);
        }

        void myLocation_Click(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Siin ma olengi :)");
            Pushpin pin = (Pushpin)sender;
            pin.Content = "Juuuuhuuuu!";
        }

        Pushpin addLocationPin(double? latitude, double? longitude, object content)
        {
            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate((double) latitude, (double) longitude);
            pin.Content = content;
            map1.Children.Add(pin);
            return pin;
        }
    }
}