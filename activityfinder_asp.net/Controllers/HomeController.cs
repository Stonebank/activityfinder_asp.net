using activityfinder_asp.net.Models;
using activityfinder_asp.net.Models.Activities.Category;
using activityfinder_asp.net.Models.Activities.Category.Container;
using activityfinder_asp.net.Models.Dto;
using activityfinder_asp.net.Models.Location;
using activityfinder_asp.net.Security;
using activityfinder_asp.net.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace activityfinder_asp.net.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            return Redirect("Discover");
        }

        public IActionResult Discover(string coords)
        {
            var lat = 1.1;
            var lon = 1.1;

            if (coords is not null)
            {
                lat = Convert.ToDouble(coords.Split(" ")[0].Replace(".", ","));
                lon = Convert.ToDouble(coords.Split(" ")[1].Replace(".", ","));
            }

           
            UserLocation userLocation = new UserLocation(new Coordinate(lat, lon));

            List<Category> categories = new List<Category>();
            categories.Add(new BestDistance());
            categories.Add(new BestWeather());

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
                Models.Activities.Activity.activities = Models.Activities.Activity.activities.OrderBy(o => o.Points).ToList();

            }

            return View(userLocation);
        }

        [HttpPost]
        public void FetchCoordinates(string coords)
        {
            var lat = Convert.ToDouble(coords.Split(" ")[0].Replace(".", ","));
            var lon = Convert.ToDouble(coords.Split(" ")[1].Replace(".", ","));

            //Debug.WriteLine("{0}, {0}", lat, lon);
  
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
            // password requirement check
            // send email verification
            account.Password = AES256.Encrypt(account.Password);
            accountHandler.Save(account);
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

            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}