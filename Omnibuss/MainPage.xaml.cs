﻿using System;
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
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher watcher;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

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

                addLocationPin(47.676289396624654, -122.12096571922302, new Uri("http://www.clker.com/cliparts/e/d/9/9/1206572112160208723johnny_automatic_NPS_map_pictographs_part_67.svg.med.png"));
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
            addLocationPin(e.Position.Location.Latitude, e.Position.Location.Longitude, new Uri("mylocation.png", UriKind.Relative));
        }

        void addLocationPin(double latitude, double longitude, Uri uri)
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
            pin.Content = new Ellipse()
            {
                Fill = image,
                StrokeThickness = 10,
                Height = 50,
                Width = 50
            };

            //---add the pushpin to the map---
            map1.Children.Add(pin);
        }
    }
}