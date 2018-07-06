using PS.Core.Entities.Other;
using PS.Core.Entities.Owner;
using PS.Core.Entities.User;
using PS.Core.Service.Services;
using PS.Web.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace integratingViews.Controllers
{
    public class UsersController : Controller
    {
        //Newly Added..
        public ActionResult Search(int id)
        {
            PlaceService service = new PlaceService();
            ParkingPlace matches = new ParkingPlace();

            FacilityService fs = new FacilityService();

            string dsc = fs.getFacility(id);

            matches = service.getPlaceInfo(id);
            ReviewService rs = new ReviewService();
            CompareViewModel data = new CompareViewModel()
            {
                Name = matches.SpotName,
                PlaceId = matches.ID,
                Price = matches.PricePerHour,
                Rating = rs.getRating(id),
                Facilities = dsc
            };

            string name = "matches";
            int _cnt = Convert.ToInt32(Session["cnt"]);

            int lim = Convert.ToInt32(Session["lim"]);
            if (lim >= 4) return RedirectToAction("compare", "users");

            _cnt++; Session["cnt"] = _cnt.ToString();

            lim++; Session["lim"] = lim.ToString();

            Session[name+_cnt.ToString()] = data;

            //List<ParkingPlace> model = new List<ParkingPlace>();
            //for(int i=1;i<=_cnt;i++)
            //{
            //    if ((Session[name + i.ToString()]).ToString() == "none") continue;
            //    ParkingPlace p = (ParkingPlace)Session[name + i.ToString()];
            //    model.Add(p);
            //}
            //return View("compare", model);
            return RedirectToAction("compare","users");
        }

        public string getSearchItems(string id)
        {
            PlaceService service = new PlaceService();
            List<ParkingPlace> matches = new List<ParkingPlace>();
            matches = service.getMatchedPlaces(id);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(matches);
            return json;
        }
        public ActionResult removeItem(int id)
        {
            string name = "matches";
            int _cnt = Convert.ToInt32(Session["cnt"]);

            int lim = Convert.ToInt32(Session["lim"]);

            List<CompareViewModel> model = new List<CompareViewModel>();
            for (int i = 1; i <= _cnt; i++)
            {
                if ((Session[name + i.ToString()]).ToString() == "none") continue;
                CompareViewModel p = (CompareViewModel)Session[name + i.ToString()];
                if(p.PlaceId == id)
                {
                    Session[name + i.ToString()] = "none";
                    lim--; Session["lim"] = lim.ToString();
                    continue;
                }
                model.Add(p);
            }
            //return View("compare", model);
            return RedirectToAction("compare", "users");
        }


        // GET: User
        public ActionResult Index()
        {
            if (Session["type"].ToString() == "1")
            {
                List<ParkingPlace> data;
                PlaceService service = new PlaceService();
                data = service.getAllPlaces();
                AdminService ads = new AdminService();
                List<Bill> bils = new List<Bill>();

                int id = Convert.ToInt32(Session["USERID"]);
                bils = ads.getAllBillsUser(id);

                ParkingPlace plc1 = new ParkingPlace();
                ParkingPlace plc2 = new ParkingPlace();
                PlaceService ps = new PlaceService();

                ViewBag.plcCnt = 0;

                if (bils.Count >= 1)
                {
                    plc1 = ps.getPlaceInfo(bils[0].PlaceId);
                    ViewBag.plcCnt = 1;
                    ViewBag.PlaceName1 = plc1.SpotName;
                    ViewBag.PlacePrice1 = plc1.PricePerHour;
                    ViewBag.PlaceId1 = plc1.ID;

                }
                if (bils.Count >= 2)
                {
                    plc2 = ps.getPlaceInfo(bils[1].PlaceId);
                    ViewBag.plcCnt = 2;
                    ViewBag.PlaceName2 = plc2.SpotName;
                    ViewBag.PlacePrice2 = plc2.PricePerHour;
                    ViewBag.PlaceId2 = plc2.ID;
                }

                OfferService os = new OfferService();
                List<Offer> offer = new List<Offer>();
                List<Promo> promo = new List<Promo>();

                Offer of = new Offer();
                Promo pr = new Promo();

                if (offer.Count >= 1)
                {
                    of = offer[0];
                    ViewBag.plcCnt = 3;

                    int pid = of.PlaceId;
                    string name = service.getPlaceInfo(pid).SpotName;
                    ViewBag.OfferName = name;
                    ViewBag.OfferPrice = of.ParkingRate;

                }
                if (promo.Count >= 1)
                {
                    pr = promo[0];
                    ViewBag.plcCnt = 4;

                    int pid = pr.PlaceId;
                    string name = service.getPlaceInfo(pid).SpotName;
                    ViewBag.PromoName = name;
                    ViewBag.PromoCode = pr.PromoCode;
                }

                return View("home", data);
            }
            else
            {
               

                return RedirectToAction("index", "Login");
            }
            
        }
        [HttpPost]
        public ActionResult Bookings(int nal)
        {

            if (Session["type"].ToString() == "1")
            {
                return View("bookings");
            }
            else
            {
                return RedirectToAction("index", "Login");
            }

        }
        [HttpGet]
        public ActionResult Bookings()
        {
            List<Booking> books = new List<Booking>();
            UserBookingService bservice = new UserBookingService();
            int id = Convert.ToInt32(Session["USERID"]);
            books = bservice.getAllBookings(id);

            //return books[0].PlaceId.ToString();

            List<Subscriptions> sub = new List<Subscriptions>();
            sub = bservice.GetAllSubscriptions(id);
            List<UserBookingModel> data = new List<UserBookingModel>();

            PlaceService pserv = new PlaceService();
            for(int i=0; i<books.Count; i++)
            {
                ParkingPlace pl = new ParkingPlace();
                pl = pserv.getPlaceInfo(books[i].PlaceId); //return pl.SpotName.ToString();
                UserBookingModel mod = new UserBookingModel();

                    mod.Name = pl.SpotName;
                    mod.ID = books[i].ID;
                    mod.PlaceId = pl.ID;
                    mod.IsPending = books[i].IsPending;
                    mod.Price = pl.PricePerHour;
                    mod.idBook = 1; //means book type

                
                data.Add(mod);
            }

            
            for (int i = 0; i < sub.Count; i++)
            {
                ParkingPlace pl = new ParkingPlace();
                pl = pserv.getPlaceInfo(sub[i].PlaceId);
                UserBookingModel mod = new UserBookingModel();


                mod.Name = pl.SpotName;
                mod.ID = sub[i].ID;
                mod.PlaceId = pl.ID;
                mod.Price = pl.PricePerHour;
                mod.IsPending = sub[i].IsPending;
                mod.idBook = 0; //means subs type
                mod.Date = sub[i].End;
                
                data.Add(mod);
            }
            //return "none";
            return View("bookings", data);
            
        }
        public ActionResult ChangePass()
        {
            if (Session["type"].ToString() == "1")
                return View("changepass");
            else
            {
                return RedirectToAction("index", "Login");
            }
            
        }
        public ActionResult Edit()
        {
            if (Session["type"].ToString() == "1")
                return View("edit");
            else
            {
                return RedirectToAction("index", "Login");
            }
            
        }
        public ActionResult Compare()
        {
            if (Session["type"].ToString() == "1")
            {
                string name = "matches";
                int _cnt = Convert.ToInt32(Session["cnt"]);

                List<CompareViewModel> model = new List<CompareViewModel>();
                for (int i = 1; i <= _cnt; i++)
                {
                    if ((Session[name + i.ToString()]).ToString() == "none") continue;
                    CompareViewModel p = (CompareViewModel)Session[name + i.ToString()];
                    model.Add(p);
                }
                return View("compare", model);
            }
            else
            {
                return RedirectToAction("index", "Login");
            }

        }
        public ActionResult _Profile()
        {
            if (Session["type"].ToString() == "1")
                return View("profile");
            else
            {
                return RedirectToAction("index", "Login");
            }
            
        }

        public ActionResult reviews()
        {
            //return Session["USERID"].ToString();
            if (Session["type"].ToString() == "1")
            {
                ReviewService service = new ReviewService();
                List<Review> revs = new List<Review>();
                int id = Convert.ToInt32(Session["USERID"]);
                revs = service.getAllReviews(id);

                List<ReviewViewModel> res = new List<ReviewViewModel>();


                AuthService atsr = new AuthService();
                for (int i = 0; i < revs.Count; i++)
                {
                    ReviewViewModel tmp = new ReviewViewModel();
                    tmp.Time = revs[i].Time;
                    tmp.Rating = revs[i].Rating;
                    tmp.Comment = revs[i].Comment;

                    if (revs[i].FromUserId == id)
                    {
                        tmp.IsToMe = 0;
                        tmp.Who = atsr.getUsername(revs[i].ToUserId);
                    }
                    else
                    {
                        tmp.IsToMe = 1;
                        tmp.Who = atsr.getUsername(revs[i].FromUserId);
                    }

                    res.Add(tmp);
                }

                List<PlaceReview> prev = new List<PlaceReview>();
                prev = service.getAllPlaceReviews(id);

                PlaceService ps = new PlaceService();
                for (int i = 0; i < prev.Count; i++)
                {
                    ReviewViewModel tmp = new ReviewViewModel();
                    tmp.Time = prev[i].Time;
                    tmp.Rating = prev[i].Rating;
                    tmp.Comment = prev[i].Comment;

                    tmp.IsToMe = 0;
                    tmp.Who = ps.getPlaceInfo(prev[i].ToPlaceId).SpotName;

                    res.Add(tmp);
                }


                return View("reviews", res);
            }

            else
            {
                return RedirectToAction("index", "Login");
            }

        }

        public ActionResult CancelRequest(int id)
        {
            OwnerBookingManagementService service = new OwnerBookingManagementService();
            service.cancelRequest(id);
            return RedirectToAction("bookings", "Users");
        }
        public ActionResult CancelSub(int id)
        {
            OwnerBookingManagementService service = new OwnerBookingManagementService();
            service.cancelSubs(id);
            return RedirectToAction("bookings", "Users");
        }

        //public ActionResult MakePlaceReview(PlaceReview rev)
        //{
        //    rev.CarUserId = Convert.ToInt32(Session["USERID"]);
        //    ReviewService service = new ReviewService();
        //    service.createPlaceReview(rev);

        //    return RedirectToAction("reviews","Users");
        //}
        public ActionResult MakeRequestById(int id)
        {
            Booking book = new Booking()
            {
                PlaceId = id,
                PromoCode = "N/A"
            };

            book.UserId = Convert.ToInt32(Session["USERID"]);
            book.IsPending = 1;

            PlaceService service = new PlaceService();
            service.RequestPlace(book);
            return RedirectToAction("index", "Users");
        }
        public ActionResult MakeRequest(Booking book)
        {
            book.UserId = Convert.ToInt32(Session["USERID"]);
            book.IsPending = 1;

            PlaceService service = new PlaceService();
            service.RequestPlace(book);
            return RedirectToAction("index", "Users");
        }
        public ActionResult MakeSubscribe(Subscriptions sub)
        {
            sub.UserId = Convert.ToInt32(Session["USERID"]);
            sub.IsPending = 1;

            //return "OK: " + sub.UserId + " " + sub.PlaceId + " " + sub.Start +" "+ sub.End + " " + sub.Date;

            PlaceService service = new PlaceService();
            service.SubscribePlace(sub);
            return RedirectToAction("index", "Users");
        }

        [HttpPost]
        public ActionResult Signup(UserRegistration form)
        {
                if (form.Password != form.ConfirmPassword)
                {
                    ViewBag.msg = "Password does not match";
                    return RedirectToAction("userSignup", "Login");
                }


                LogInInfo log = new LogInInfo
                {
                    Username = form.Username,
                    Password = form.Password,
                    Type = 1,
                    IsBlocked = 0
                };

                User usr = new User
                {
                    Mobile = form.Mobile,
                    Email = form.Email,
                    CarModel = form.CarModel,
                    LicensNumber = form.LicensNumber
                };

                UserService service = new UserService();

                if (service.register(log, usr))
                    return RedirectToAction("index", "Login");
                else
                {
                    ViewBag.msg2 = "Username Already Exists";
                    return RedirectToAction("userSignup", "Login");
                }
        }
        public ActionResult GetAllBooking()
        {
            IEnumerable<Booking> places = new List<Booking>();
            if (Session["type"].ToString() == "1")
            {
                UserBookingService BService = new UserBookingService();
                int id = Convert.ToInt32(Session["USERID"]);
                places = BService.getAllBookings(id);
                return View("home", places);
            }

            else
            {
                return RedirectToAction("index", "Login");
            }
        }
        public ActionResult GetAllSubscription()
        {
            List<Subscriptions> places = new List<Subscriptions>();
            if (Session["type"].ToString() == "1")
            {
                UserBookingService BService = new UserBookingService();
                int id = Convert.ToInt32(Session["USERID"]);
                places = BService.GetAllSubscriptions(id);
                return View("home", places);
            }

            else
            {
                return RedirectToAction("index", "Login");
            }
        }

        public string Search2(string id)
        {

            PlaceService service = new PlaceService();
            List<ParkingPlace> matches = new List<ParkingPlace>();
            matches = service.getMatchedPlaces(id);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(matches);
            return json;

        }

        public ActionResult MakePlaceReview(PlaceReview rev)
        {
            int bookId = rev.ID;
            rev.CarUserId = Convert.ToInt32(Session["USERID"]);
            ReviewService service = new ReviewService();
            service.createPlaceReview(rev);

            
            //To isPending 2
            OwnerBookingManagementService obs = new OwnerBookingManagementService();
            obs.changePendingBook(bookId);

            return RedirectToAction("reviews", "Users");
        }

        public ActionResult MakeSubReview(PlaceReview rev)
        {
            int subId = rev.ID;

            rev.CarUserId = Convert.ToInt32(Session["USERID"]);
            ReviewService service = new ReviewService();
            service.createPlaceReview(rev);

            //To isPending 2
            OwnerBookingManagementService obs = new OwnerBookingManagementService();
            obs.changePendingSubs(subId);

            return RedirectToAction("reviews", "Users");
        }
        public ActionResult ShowBills()
        {
            List<Bill> bills = new List<Bill>();
            AdminService ads = new AdminService();

            int id = Convert.ToInt32(Session["USERID"]);
            bills = ads.getAllBillsUser(id);

            return View("Bills", bills);
        }
    }
}