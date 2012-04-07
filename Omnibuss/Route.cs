using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Omnibuss
{
    public class Route
    {
        public Route(int id, String number, String name)
        {
            Id = id;
            Number = number;
            Name = name;
        }

        public int Id
        {
            get;
            set;
        }

        public String Number
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }
    }
}
