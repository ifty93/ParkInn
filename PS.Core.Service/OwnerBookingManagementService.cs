using PS.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS.Core.Entities.Other;
using PS.Infrastructure;
using PS.Core.Entities.Owner;
using PS.Core.Entities.User;

namespace PS.Core.Service.Services
{
    public class OwnerBookingManagementService : IOwnerBookingManagementService
    {
        public bool changePendingBook(int id)
        {
            PsDbContex db = new PsDbContex();
            Booking el = db.Bookings.SingleOrDefault(r => r.ID == id);
            if (el == null) return false; el.IsPending = 2; db.SaveChanges();
            return true;
        }
        public bool changePendingSubs(int id)
        {
            PsDbContex db = new PsDbContex();
            Subscriptions el = db.Subscriptions.SingleOrDefault(r => r.ID == id);
            if (el == null) return false; el.IsPending = 2; db.SaveChanges();
            return true;
        }
        //pending = 0  and  active = 1
        public bool acceptRequest(int aRequestId)
        {
            // Caution: Update May Not Work..
            PsDbContex db = new PsDbContex();
            List<Booking> data;

            try
            {
                var el = from r in db.Bookings
                         where r.ID == aRequestId
                         select r;
                data = el.ToList();
            }
            catch
            {
                return false;
            }

            if (data.Count != 1) return false;
            data[0].IsPending = 0;
            db.SaveChanges();

            return true;
        }

        public bool cancelRequest(int aRequestId)
        {
            PsDbContex db = new PsDbContex();
            List<Booking> data;

            try
            {
                var el = from r in db.Bookings
                         where r.ID == aRequestId
                         select r;
                data = el.ToList();

                if (data.Count != 1) return false;
                db.Bookings.Remove(data[0]);
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool cancelSubs(int aSubId)
        {
            PsDbContex db = new PsDbContex();
            List<Subscriptions> data;

            try
            {
                var el = from r in db.Subscriptions
                         where r.ID == aSubId
                         select r;
                data = el.ToList();

                if (data.Count != 1) return false;
                db.Subscriptions.Remove(data[0]);
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Booking> getAllBookings(int aOwnerId)
        {
            PsDbContex db = new PsDbContex();
            List<ParkingPlace> placeData;

            try
            {
                var el = from r in db.ParkingPlaces
                         where r.OwnerId == aOwnerId
                         select r;
                placeData = el.ToList();
            }
            catch
            {
                return null;
            }

            /**********************************************************************/

            List<Booking> data = new List<Booking>();

            for (int i = 0; i < placeData.Count; i++)
            {
                int placeId = placeData[i].ID;

                List<Booking> cur = new List<Booking>();
                var el = from r in db.Bookings
                         where r.PlaceId == placeId
                         select r;
                cur = el.ToList();
                //Booking el = db.Bookings.SingleOrDefault(r => r.PlaceId == placeId);
                //if(el != null) data.Add(el);
                if (el == null || cur.Count == 0) continue;

                for (int k = 0; k < cur.Count; k++) data.Add(cur[k]);
            }

            return data;
        }
        public List<Subscriptions> getAllSubscriptions(int aOwnerId)
        {
            PsDbContex db = new PsDbContex();
            List<ParkingPlace> placeData;

            try
            {
                var el = from r in db.ParkingPlaces
                         where r.OwnerId == aOwnerId
                         select r;
                placeData = el.ToList();
            }
            catch
            {
                return null;
            }

            /**********************************************************************/

            List<Subscriptions> data = new List<Subscriptions>();

            for (int i = 0; i < placeData.Count; i++)
            {
                int placeId = placeData[i].ID;

                Subscriptions el = db.Subscriptions.SingleOrDefault(r => r.PlaceId == placeId);
                if (el != null) data.Add(el);
            }

            return data;
        }

        public List<Booking> getAllRequests(int aOwnerId)
        {
            PsDbContex db = new PsDbContex();
            List<ParkingPlace> placeData;

            try
            {
                var el = from r in db.ParkingPlaces
                         where r.OwnerId == aOwnerId
                         select r;
                placeData = el.ToList();
            }
            catch
            {
                return null;
            }

            /**********************************************************************/

            List<Booking> data = new List<Booking>();

            for (int i = 0; i < placeData.Count; i++)
            {
                int placeId = placeData[i].ID;

                Booking el = db.Bookings.SingleOrDefault(r => r.PlaceId == placeId && r.IsPending == 0);
                if (el != null) data.Add(el);
            }

            return data;
        }

        public Booking getBooking(int id)
        {
            PsDbContex db = new PsDbContex();
            Booking el = db.Bookings.SingleOrDefault(r => r.ID == id);
            return el;
        }

    }
}
