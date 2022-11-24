using activityfinder_asp.net.Models;
using activityfinder_asp.net.Models.Activities;
using activityfinder_asp.net.Models.Activities.Category;
using activityfinder_asp.net.Models.Activities.Category.Container;
using activityfinder_asp.net.Models.Dto;
using activityfinder_asp.net.Models.Location;
using activityfinder_asp.net.Security;
using activityfinder_asp.net.Service;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace activityfinder_asp.net.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private double lat;
        private double lon;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Register(string token)
        {
            ISession session = HttpContext.Session;
            if (session.GetString("email") != null)
            {
                return Redirect("index");
            }
            if (!String.IsNullOrEmpty(token))
            {
                AccountHandler accountHandler = new AccountHandler();
                Account account = accountHandler.Load(long.Parse(token));
                if (account is null)
                {
                    TempData["Error-Message"] = "Oops... Your verification wasn't successful.";
                    return View();
                }
                account.Verified = true;
                accountHandler.Save(account);
                TempData["Successful"] = "Welcome onboard! Your account is now confirmed.";
            }

            return View();
        }

        public IActionResult Login()
        {
            ISession session = HttpContext.Session;
            if (session.GetString("email") != null)
            {
                return Redirect("index");
            }
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Discover()
        {

            ISession session = HttpContext.Session;
            if (session.GetString("email") == null)
            {
                return Redirect("Login");
            }

            Debug.WriteLine(lat + " "+  lon);

            UserLocation userLocation = new UserLocation(new Coordinate(55.7314, 12.3962));

            List<Category> categories = new List<Category>
            {
                new BestDistance(),
                new BestWeather()
            };

            categories.ForEach(c =>
            {
                c.Compare(userLocation);
                c.AddPoints();
                if (c.BestCandidate() != null)
                {
                    Debug.WriteLine("Best candidate: " + c.BestCandidate().Name);
                }
            });

            if (Models.Activities.Activity.activities is not null)
            {
                Models.Activities.Activity.activities = Models.Activities.Activity.activities.OrderBy(o => o.Points).Reverse().ToList();

            }

            return View(userLocation);
        }

        [HttpPost]
        public void FetchCoordinates(string[] coords)
        {
            var english = CultureInfo.GetCultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = english;

            lat = Double.Parse(coords[0]);
            lon = Double.Parse(coords[1]);

            Debug.WriteLine(lat + " " + lon);
        }

        [HttpPost]
        public ActionResult RegisterAccount(Account account)
        {
            AccountHandler accountHandler = new AccountHandler();

            if (accountHandler.Load(account.Email) is not null)
            {
                TempData["Error-Message"] = "The email address you entered is already in use.";
                return View("Register");
            }
            if (!account.Email.ToLower().Equals(account.RepeatEmail.ToLower()))
            {
                TempData["Error-Message"] = "The two email addresses that you entered do not match.";
                return View("Register");
            }
            if (!account.Password.Equals(account.RepeatPassword))
            {
                TempData["Error-Message"] = "The two passwords that you entered do not match.";
                return View("Register");
            }
            if (!Constant.HasPasswordRequirement(account.Password))
            {
                TempData["Error-Message"] = "Error! Password is not strong enough. (debug pass: dTu1235678!)";
                return View("Register");
            }
            account.Password = AES256.Encrypt(account.Password);
            accountHandler.SendVerificationEmail(account, Request.Host.Value, Convert.ToString(account.Id));
            accountHandler.Save(account);
            TempData["Successful"] = "Almost done! We've sent a confirmation e-mail to " + account.Email + ".";
            return View("Register");
        }

        [HttpPost] 
        public ActionResult LoginAccount(Account account)
        {
            AccountHandler accountHandler = new AccountHandler();
            Account user = accountHandler.Load(account.Email);

            if (user is null)
            {
                TempData["Error-Message"] = "E-mail address not found";
                return View("Login");
            }

            if (!user.Email.ToLower().Equals(account.Email.ToLower()))
            {
                TempData["Error-Message"] = "Login failed. Invaild e-mail or password.";
                return View("Login");
            }

            if (!AES256.Decrypt(user.Password).Equals(account.Password))
            {
                TempData["Error-Message"] = "Login failed. Invaild e-mail or password.";
                return View("Login");
            }

            if (!user.Verified)
            {
                TempData["Error-Message"] = "Please verify your account in order to login.";
                return View("Login");
            }
            ISession session = HttpContext.Session;
            session.SetString("email", account.Email);
            return View("Index");
        }

        public IActionResult Logout()
        {
            ISession session = HttpContext.Session;
            if (session.GetString("email") != null)
            {
                session.Remove("email");
            }
            return Redirect("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}