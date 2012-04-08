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
using System.IO.IsolatedStorage;
using System.Diagnostics;


namespace Omnibuss
{
    public partial class TicketPage : PhoneApplicationPage
    {

        string isikukood;
        string dokNumber;

        public TicketPage()
        {
            InitializeComponent();
            var x = IsolatedStorageSettings.ApplicationSettings.TryGetValue("isikukood", out isikukood);
            var y = IsolatedStorageSettings.ApplicationSettings.TryGetValue("dokNr", out dokNumber);
            if (isikukood == null || dokNumber == null || isikukood.Length == 0 || dokNumber.Length == 0) {
                Debug.WriteLine("Show inputs");
                DataPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Add("isikukood", idInput.Text);
            IsolatedStorageSettings.ApplicationSettings.Add("dokNr", docNrInput.Text);
            var x = IsolatedStorageSettings.ApplicationSettings.TryGetValue("isikukood", out isikukood);
            var y = IsolatedStorageSettings.ApplicationSettings.TryGetValue("dokNr", out dokNumber);
            Debug.WriteLine("Hei:" + isikukood + " / " + dokNumber + "!");
            DataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}