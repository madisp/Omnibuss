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

        public List<Route> GetRoutes()
        {
            List<Route> routes = new List<Route>();
            routes.Add(new Route(1, "3A", "Möku - Nott"));
            routes.Add(new Route(2, "1A", "Nott - Püssikas"));
            routes.Add(new Route(3, "11", "Annesaun - Nott"));
            routes.Add(new Route(4, "34", "Möku - Nõo baar"));
            routes.Add(new Route(5, "2", "Atlantis - Nõo baar"));
            routes.Add(new Route(6, "7", "Illukas - Nott"));
            routes.Add(new Route(7, "3A", "Möku - Nott"));
            routes.Add(new Route(8, "1A", "Nott - Püssikas"));
            routes.Add(new Route(9, "11", "Annesaun - Nott"));
            routes.Add(new Route(10, "34", "Möku - Nõo baar"));
            routes.Add(new Route(11, "2", "Atlantis - Nõo baar"));
            routes.Add(new Route(12, "7", "Illukas - Nott"));
            return routes;
        }
    }
}
