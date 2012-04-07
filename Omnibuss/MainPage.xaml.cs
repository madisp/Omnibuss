using System;
using System.Net;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using System.Diagnostics; //---for Debug.WriteLine()---
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Phone.Controls.Maps.Platform;

namespace Omnibuss
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher watcher;

        WebClient wc;

        // Constructor
        public MainPage()
        {


            InitializeComponent();

            // Create the WebClient and associate a handler with the OpenReadCompleted event.
            wc = new WebClient();
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
            CallToWebService("58.383333", "26.716667", "58.383333", "26.816667");

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

                map1.Center = new GeoCoordinate(58.383943, 26.717124);
                map1.ZoomLevel = 15;

                Pushpin pin = addLocationPin(47.676289396624654, -122.12096571922302, new Uri("http://www.clker.com/cliparts/e/d/9/9/1206572112160208723johnny_automatic_NPS_map_pictographs_part_67.svg.med.png"));
                pin.MouseLeftButtonUp += new MouseButtonEventHandler(myLocation_Click);
            }
        }

        private void CallToWebService(string srcLatitude, string srcLongitude, string dstLatitude, string dstLongitude)
        {
            // Call the OpenReadAsyc to make a get request, passing the url with the selected search string.
            wc.OpenReadAsync(new Uri("http://dev.virtualearth.net/REST/V1/Routes/Driving?o=xml&rpo=Points&tl=0.00001&wp.0=" + srcLatitude + "," + srcLongitude + "&wp.1=" + dstLatitude + "," + dstLongitude + "&key=Aj2gDlArPAqNxkeyI11APMNS_g_1RYAj9yJgEXxYcXQB2nU7BWTJQkACS8js5_Kr"));
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
                XNamespace web = "http://schemas.microsoft.com/search/local/ws/rest/v1";
                try
                {
                    List<RoutePoint> routePoints = new List<RoutePoint>();
                    resultXml = XElement.Load(e.Result);

                    var points = from item
                                in resultXml.Descendants(web + "Line").ElementAt(0).Descendants(web + "Point")
                                 select item;
                    foreach (XElement item in points)
                    {
                        RoutePoint result = new RoutePoint();

                        string latitude = item.Descendants(web + "Latitude").ToArray().ElementAt(0).Value;
                        string longitude = item.Descendants(web + "Longitude").ToArray().ElementAt(0).Value;

                        result.Latitude = Double.Parse(latitude);
                        result.Longitude = Double.Parse(longitude);
                        Debug.WriteLine("POINT: " + result.Latitude + ", " + result.Longitude);
                        routePoints.Add(result);
                    }


                    routeLoaded(routePoints);


                }
                catch (System.Xml.XmlException ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

        }

        void routeLoaded(List<RoutePoint> points)
        {
            // OH IT'S SO AWESOME
            MapPolyline polyLine;
            polyLine = new MapPolyline();
            polyLine.Stroke = new SolidColorBrush(Colors.Blue);
            polyLine.StrokeThickness = 5;
            polyLine.Opacity = 0.7;
            polyLine.Locations = new LocationCollection();

            foreach (RoutePoint point in points)
            {
                polyLine.Locations.Add(new GeoCoordinate(point.Latitude, point.Longitude));
            }

            map1.Children.Add(polyLine);
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
            // map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
            Pushpin pin = addLocationPin(e.Position.Location.Latitude, e.Position.Location.Longitude, new Uri("mylocation.png", UriKind.Relative));
            pin.MouseLeftButtonUp += new MouseButtonEventHandler(myLocation_Click);
        }

        void myLocation_Click(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Siin ma olengi :)");
            Pushpin pin = (Pushpin)sender;
            pin.Content = "Juuuuhuuuu!";
        }

        Pushpin addLocationPin(double latitude, double longitude, Uri uri)
        {
            Pushpin pin = new Pushpin();

            //---set the location for the pushpin---
            pin.Location = new GeoCoordinate(latitude, longitude);

            //---use an ImageBrushobject and fill it with an image from the web---
            ImageBrush image = new ImageBrush()
            {
                ImageSource = new System.Windows.Media.Imaging.BitmapImage(uri)
            };

            //---draw an ellipse inside the pushpin and fill it with the image---
            pin.Content = "tere";

            //---add the pushpin to the map---
            map1.Children.Add(pin);
            return pin;
        }
    }

    public class RoutePoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}