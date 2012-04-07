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

namespace Omnibuss
{
    public partial class RouteDetailsPanoramaPage : PhoneApplicationPage
    {
        public RouteDetailsPanoramaPage()
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

            if (!NavigationContext.QueryString.TryGetValue("routeId", out idString))
            {
                Debug.WriteLine("No route ID provided!");
                NavigationService.GoBack();
                return;
            }

            UInt32 routeId = UInt32.Parse(idString);
            Debug.WriteLine("Route id: " + routeId);

            OmnibussModel model = new OmnibussModel();
            Stop stop = model.GetStop(stopId);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}