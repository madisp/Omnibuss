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
using System.Linq;

namespace Omnibuss
{
    public class OmnibussModel
    {
        private OmnibussDataContext db;

        public OmnibussModel() {
            db = new OmnibussDataContext(OmnibussDataContext.ConnectionStringReadOnly);
        }

        public List<Stop> GetStops()
        {
            return (from Stop stop in db.Stops select stop).ToList<Stop>();
        }

        public Stop GetStop(UInt32 id)
        {
            return (from Stop stop in db.Stops where stop.Id.Equals(id) select stop).Single<Stop>();
        }

        public List<Route> GetRoutesByStop(Stop stop)
        {
            return (
                from route in db.Routes
                    join trip in db.Trips on route.Route_id equals trip.Route_id
                    join stop_time in db.Stop_times on trip.Trip_id equals stop_time.Trip_id
                select route).Distinct().ToList();
        }
    }
}
