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
        private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        string isikukood;
        string dokNumber;

        public TicketPage()
        {
            InitializeComponent();

            options.ItemsSource = TicketOption.getOptions();

            try
            {
                isikukood = (string)appSettings["isikukood"];
                dokNumber = (string)appSettings["dokNr"];
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                 Debug.WriteLine("Oh noes, no values");
            }
            if (isikukood == null || dokNumber == null || isikukood.Length == 0 || dokNumber.Length == 0)
            {
                Debug.WriteLine("Show inputs: " + isikukood + ", " + dokNumber);
                DataPanel.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            // TODO present the ticket options here
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Add("isikukood", idInput.Text);
            appSettings.Add("dokNr", docNrInput.Text);
            isikukood = (string)appSettings["isikukood"];
            dokNumber = (string)appSettings["dokNr"];
            Debug.WriteLine("Hei:" + isikukood + " / " + dokNumber + "!");
            DataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}