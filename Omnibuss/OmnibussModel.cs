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
using System.Collections;

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
                where
                    stop_time.Stop_id == stop.Id
                orderby route.Route_short_name ascending
                select route).Distinct().OrderBy(route => route.Route_short_name).ToList();
        }

        public Route GetRoute(UInt32 id)
        {
            return (from Route route in db.Routes where route.Route_id.Equals(id) select route).Single();
        }

        public List<Trip> GetTripsByRoute(Route route)
        {
            return (from trip in db.Trips where trip.Route_id.Equals(route.Route_id) select trip).ToList();
        }

        public Trip GetMaxTripByRoute(Route route)
        {
            var trips =
                from trip in db.Trips
                join stop_time in db.Stop_times on trip.Trip_id equals stop_time.Trip_id into j1
                from j2 in j1.DefaultIfEmpty()
                group j2 by trip.Trip_id into grouped
                orderby grouped.Count() descending
                select new { ParentId = grouped.Key, Count = grouped.Count() };

            return (from trip in db.Trips where trip.Trip_id.Equals(trips.Single().ParentId) select trip).Single();
        }

        public List<Stop> GetStopsByTrip(Trip trip)
        {
            List<Point> stopTimes = (
                from stop_time in db.Stop_times
                join stop in db.Stops on stop_time.Stop_id equals stop.Id
                where stop_time.Trip_id.Equals(trip.Trip_id)
                select new Point { StopId = stop.Id, Sequence = stop_time.Stop_sequence, Latitude = stop.Latitude, Longitude = stop.Longitude }
            ).Distinct().OrderBy(pt => pt.Sequence).ToList();

            List<Stop> ret = new List<Stop>();

            foreach (Point time in stopTimes)
            {
                ret.Add(GetStop((uint)time.StopId));
            }

            return ret;
        }
    }

    internal class Comparer : IComparer<Point>
    {
        int System.Collections.Generic.IComparer<Point>.Compare(Point x, Point y)
        {
            throw new NotImplementedException();
        }
    }

    internal class ProjectionComparer<TElement, TKey> : IComparer<TElement>
    {
        private readonly Func<TElement, TKey> keySelector;
        private readonly IComparer<TKey> comparer;

        internal ProjectionComparer(Func<TElement, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            this.keySelector = keySelector;
            this.comparer = comparer ?? Comparer<TKey>.Default;
        }

        public int Compare(TElement x, TElement y)
        {
            TKey keyX = keySelector(x);
            TKey keyY = keySelector(y);
            return comparer.Compare(keyX, keyY);
        }
    }

    public class Point : IEquatable<Point>
    {
        public int? StopId;
        public int? Sequence;
        public double? Latitude;
        public double? Longitude;

        public bool Equals(Point other)
        {
            return StopId == other.StopId;
        }
    }
}
