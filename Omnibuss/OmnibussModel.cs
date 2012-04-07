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
using System.Collections.Generic;
using System.Data.Linq;

namespace Omnibuss
{
    public class OmnibussModel
    {
        public List<Stop> GetStops()
        {
            List<Stop> ret = new List<Stop>();
            using (OmnibussDataContext db = new OmnibussDataContext(OmnibussDataContext.ConnectionStringReadOnly))
            {
                var stops = db.Stops;
                IEnumerator<Stop> enumer = stops.GetEnumerator();
                while (enumer.MoveNext())
                {
                    ret.Add(enumer.Current);
                }
            }
            return ret;
        }

        public Stop GetStop(UInt32 id)
        {
            foreach (Stop stop in GetStops())
            {
                if (stop.Id == id)
                {
                    return stop;
                }
            }
            return null;
        }
    }
}
