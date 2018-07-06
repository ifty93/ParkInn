using PS.Core.Entities.Other;
using PS.Core.Entities.User;
using PS.Core.Service.Interface;
using PS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Core.Service.Services
{
    public class AdminService : IAdminService
    {
        public bool registerAdmin()
        {
            LogInInfo newUser = new LogInInfo
            {
                ID = 10,
                Username = "ift_admin",
                Password = "admin",
                Type = 3,
                IsBlocked = 0
            };

            PsDbContex db = new PsDbContex();
            db.LogInfos.Add(newUser);
            db.SaveChanges();

            return true;
        }
        public int createBill(Bill aBill)
        {
            PsDbContex db = new PsDbContex();
            var x = db.Bills.Add(aBill);
            db.SaveChanges();

            return x.ID;
        }
        public List<Bill> getAllBillsUser(int id)
        {
            PsDbContex db = new PsDbContex();
            List<Bill> data;

            try
            {
                var el = from r in db.Bills
                         where r.UserId == id
                         select r;
                data = el.ToList();
            }
            catch
            {
                return null;
            }

            return data;
        }
        public List<Bill> getAllBillsOwner(int id)
        {
            PsDbContex db = new PsDbContex();
            List<Bill> data;

            try
            {
                var el = from r in db.Bills
                         where r.OwnerId == id
                         select r;
                data = el.ToList();
            }
            catch
            {
                return null;
            }

            return data;
        }
    }
}
