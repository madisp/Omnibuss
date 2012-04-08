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
using Microsoft.Phone.Tasks;



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
                Debug.WriteLine("Oh noes, no values: " + ex);
            }
            if (isikukood == null || dokNumber == null || isikukood.Length == 0 || dokNumber.Length == 0)
            {
                Debug.WriteLine("Show inputs: " + isikukood + ", " + dokNumber);

                DataPanel.Visibility = System.Windows.Visibility.Visible;
                options.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            // TODO present the ticket options here
            options.MouseLeftButtonDown += new MouseButtonEventHandler(
                        (object sender, MouseButtonEventArgs e) =>
                        {
                            PhoneCallTask callTask = new PhoneCallTask();
                            callTask.PhoneNumber = "999999";
                            callTask.DisplayName = "debugMode";
                            callTask.Show();

                        });


            this.Loaded += new RoutedEventHandler(TicketPage_Loaded);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //Ilmumine.Begin();
        }

        void TicketPage_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Add("isikukood", idInput.Text);
            appSettings.Add("dokNr", docNrInput.Text);
            isikukood = (string)appSettings["isikukood"];
            dokNumber = (string)appSettings["dokNr"];
            Debug.WriteLine("Hei:" + isikukood + " / " + dokNumber + "!");
            DataPanel.Visibility = System.Windows.Visibility.Collapsed;
            options.Visibility = System.Windows.Visibility.Visible;
        }
    }
}