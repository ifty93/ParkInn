using PS.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS.Core.Entities.Owner;
using PS.Infrastructure;

namespace PS.Core.Service.Services
{
    public class OfferService : IOfferService
    {
        public bool createOffer(Offer aOffer)
        {
            PsDbContex db = new PsDbContex();
            db.Offers.Add(aOffer);
            db.SaveChanges();

            return true;
        }

        public bool createPromo(Promo aPromo)
        {
            PsDbContex db = new PsDbContex();
       

            db.Promos.Add(aPromo);
            db.SaveChanges();

            return true;
        }

        public Promo getPromo(int pId, string code)
        {
            PsDbContex db = new PsDbContex();
            Promo df = new Promo()
            {
                ID = 0,
                DiscountRate = 0,
                ExpireDate = DateTime.Today
            };
            Promo el = db.Promos.SingleOrDefault(r => r.PlaceId == pId && r.PromoCode == code);
            if (el == null) return df; else return el;
        }

        public List<Promo> getAllPromo()
        {
            PsDbContex db = new PsDbContex();

            List<Promo> data = new List<Promo>();

            var el = from r in db.Promos
                     select r;
            data = el.ToList();

            return data;
        }
        public List<Offer> getAllOffer()
        {
            PsDbContex db = new PsDbContex();
            Offer df = new Offer()
            {
                ID = 0,
                ParkingRate = 0,
                FixDiscount = 0,
                ExpireDate = DateTime.Today
            };

            List<Offer> data = new List<Offer>();

            var el = from r in db.Offers
                     select r;
            data = el.ToList();

            return data;
        }
        public Offer getOffer(int pId)
        {
            PsDbContex db = new PsDbContex();
            Offer df = new Offer()
            {
                ID = 0,
                ParkingRate = 0,
                FixDiscount = 0,
                ExpireDate = DateTime.Today
            };
            
            List<Offer> data= new List<Offer>();

            var el = from r in db.Offers
                     where r.PlaceId == pId
                     select r;
            data = el.ToList();

            if (data == null || data.Count==0) return df; else return data[0];
        }
    }
}
