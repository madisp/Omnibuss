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

        public List<Stop_time> GetTimesByRouteAndStop(Route route, Stop stop)
        {

            /* no dynamic cols, too tired to change the schema. 8AM. -madis */
            var query = (
                from stop_time in db.Stop_times
                join trip in db.Trips on stop_time.Trip_id equals trip.Trip_id
                join service in db.Services on trip.Service_id equals service.Service_id
                where trip.Route_id == route.Route_id && stop_time.Stop_id == stop.Id
                orderby stop_time.Departure_time ascending
                select new { Time = stop_time, Service = service }
            );

            List<Stop_time> timepts = new List<Stop_time>();

            switch (DateTime.Today.DayOfWeek)
            {
            case DayOfWeek.Monday:
                    var times = query.Where(o => o.Service.Monday == 1);
                    foreach (var v in times.ToList()) {
                        timepts.Add(v.Time);
                    }
                    break;
            case DayOfWeek.Tuesday:
                    times = query.Where(o => o.Service.Tuesday == 1);
                    foreach (var v in times.ToList())
                    {
                        timepts.Add(v.Time);
                    }
                    break;
            case DayOfWeek.Wednesday:
                    times = query.Where(o => o.Service.Wednesday == 1);
                    foreach (var v in times.ToList())
                    {
                        timepts.Add(v.Time);
                    }
                    break;
            case DayOfWeek.Thursday:
                    times = query.Where(o => o.Service.Thursday == 1);
                    foreach (var v in times.ToList())
                    {
                        timepts.Add(v.Time);
                    }
                    break;
            case DayOfWeek.Friday:
                    times = query.Where(o => o.Service.Friday == 1);
                    foreach (var v in times.ToList())
                    {
                        timepts.Add(v.Time);
                    }
                    break;
            case DayOfWeek.Saturday:
                    times = query.Where(o => o.Service.Saturday == 1);
                    foreach (var v in times.ToList())
                    {
                        timepts.Add(v.Time);
                    }
                    break;
            case DayOfWeek.Sunday:
                    times = query.Where(o => o.Service.Sunday == 1);
                    foreach (var v in times.ToList())
                    {
                        timepts.Add(v.Time);
                    }
                    break;
            }
            return timepts;
        }

        public Trip GetMaxTripByRoute(Route route, int direction)
        {
            var trips =
                (from trip in db.Trips
                let sCount = (
                    from stop_time in db.Stop_times
                    where stop_time.Trip_id == trip.Trip_id
                    select stop_time.Stop_id
                ).Count()
                where trip.Route_id == route.Route_id && trip.Direction == direction
                orderby sCount descending
                select new { Trip = trip, Count = sCount }).Take(1).SingleOrDefault();
            
            return trips == null? null : trips.Trip;
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
