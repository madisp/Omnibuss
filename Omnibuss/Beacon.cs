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
    public class Beacon
    {

        public virtual int Id { get; set; }
        public virtual DateTime Timestamp { get; set; }
        public virtual string Description { get; set; }
        public virtual string Type { get; set; }
        public virtual double DistanceFromCustomer { get; set; }

        public override string ToString()
        {
            return Timestamp + " " + Description;
        }
    }
}
