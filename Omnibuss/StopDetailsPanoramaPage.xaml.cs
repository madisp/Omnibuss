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

namespace Omnibuss
{
    public partial class StopDetailsPanoramaPage : PhoneApplicationPage
    {
        public StopDetailsPanoramaPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            String idString = "";
            if (!NavigationContext.QueryString.TryGetValue("stopId", out idString))
            {
                Debug.WriteLine("No stop ID provided!");
                NavigationService.GoBack();
                return;
            }
            UInt32 stopId = UInt32.Parse(idString);
            Debug.WriteLine("Stop id: " + stopId);

            OmnibussModel model = new OmnibussModel();
            Stop stop = model.GetStop(stopId);
            Panorama.Title = stop.Name;

            routeList.ItemsSource = model.GetRoutes();

            addLocationPin(stop.Latitude, stop.Longitude, stop.Name);
            map1.Center = new GeoCoordinate((double)stop.Latitude, (double)stop.Longitude);
        }

        Pushpin addLocationPin(double? latitude, double? longitude, object content)
        {
            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate((double)latitude, (double)longitude);
            pin.Content = content;
            map1.Children.Add(pin);
            return pin;
        }
    }
}