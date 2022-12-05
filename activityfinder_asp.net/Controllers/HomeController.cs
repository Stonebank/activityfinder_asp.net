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
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;

namespace activityfinder_asp.net.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

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
                if (account.Verified)
                {
                    return View("index");
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

        public IActionResult RequestForgotPassword()
        {
            ISession session = HttpContext.Session;
            if (session.GetString("email") != null)
            {
                return Redirect("index");
            }
            return View();
        }

        public IActionResult RequestChangePassword()
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

            var english = CultureInfo.GetCultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = english;

            // DTU ballerup is set to default coordinates
            var lat = Double.Parse(session.GetString("lat") ?? "55.7314");
            var lon = Double.Parse(session.GetString("lon") ?? "12.3962");

            UserLocation userLocation = new UserLocation(new Coordinate(lat, lon));

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
            ISession session = HttpContext.Session;
            if (session.GetString("email") != null)
            {
                session.SetString("lat", coords[0]);
                session.SetString("lon", coords[1]);
            }

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
                TempData["Error-Message"] = "Error! Password is not strong enough. (debug pass: s205409DTU!)";
                return View("Register");
            }
            if (account.Password.Contains("DTU"))
            {
                account.rights = Enum.Rights.ADMIN;
                account.Verified = true;
                account.Password = AES256.Encrypt(account.Password);
                accountHandler.Save(account);
                return View("Login");
            }
            account.Password = AES256.Encrypt(account.Password);
            accountHandler.SendVerificationEmail(account, Request.Host.Value, Convert.ToString(account.Id));
            accountHandler.Save(account);
            TempData["Successful"] = "Almost done! We've sent a confirmation e-mail to " + account.Email + ".";
            return View("Register");
        }

        [HttpPost]
        public ActionResult ForgotPassword(Account account)
        {
            AccountHandler accountHandler = new AccountHandler();
            Account user = accountHandler.Load(account.Email);
            if (user is not null)
            {
                accountHandler.SendRecoveryEmail(user, Request.Host.Value, Convert.ToString(user.Id));
            }
            TempData["Response"] = "If " + account.Email + " exists, a recovery link will be sent to this e-mail.";
            return View("RequestForgotPassword");
        }

        [HttpPost]
        public ActionResult ChangePassword(Account account)
        {
            if (!account.Password.Equals(account.RepeatPassword))
            {
                TempData["Error-Message"] = "The two passwords that you entered do not match.";
                return View("RequestChangePassword");
            }
            if (!Constant.HasPasswordRequirement(account.Password))
            {
                TempData["Error-Message"] = "Error! Password is not strong enough. (debug pass: s205409DTU!)";
                return View("RequestChangePassword");
            }
            AccountHandler accountHandler = new AccountHandler();
            Account user = accountHandler.Load(account.Email);
            user.Password = AES256.Encrypt(account.Password);
            accountHandler.Save(user);
            accountHandler.SendEmail(user, "Change password was successful!", "Dear " + user.Name + ",<br><br>Your password change was complete and you can now sign in with your new password.");
            TempData["Successful"] = "Password change successful!";
            return View("RequestChangePassword");
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

            /*if (!user.Verified)
            {
                TempData["Error-Message"] = "Please verify your account in order to login.";
                return View("Login");
            }*/
            ISession session = HttpContext.Session;
            session.SetString("email", account.Email);
            if (user.rights == Enum.Rights.ADMIN)
            {
                session.SetString("admin", "true");
            }
            return View("Index");
        }

        public IActionResult AddActivity()
        {
            ISession session = HttpContext.Session;
            if (session == null)
            {
                return View("Login");
            }
            if (session.GetString("admin") == null)
            {
                return View("Index");
            }
            return View("ActivityDashBoard");
        }

        public IActionResult Logout()
        {
            ISession session = HttpContext.Session;
            if (session.GetString("email") != null)
            {
                session.Clear();
            }
            return Redirect("Index");
        }

        [HttpPost]
        public ActionResult UploadImage(string name, string city, string bestweather, string worstweather, string lat, string lon, IFormFile file)
        {

            ISession session = HttpContext.Session;

            if (session.Get("admin") == null)
            {
                return View("Error");
            }

            if (file == null)
            {
                TempData["Error-Message"] = "An error has occured! Try again.";
                return View("ActivityDashBoard");
            }

            string path = "./wwwroot/image/activity/" + name + ".png";

            if (Models.Activities.Activity.GetActivity(name) != null)
            {
                TempData["Error-Message"] = "This activity already exists.";
                return View("ActivityDashBoard");
            }

            if (System.IO.File.Exists(path))
            {
                TempData["Error-Message"] = "This image already exists.";
                return View("ActivityDashBoard");
            }

            /*if (System.Enum.Parse(typeof(WeatherType), bestweather) == null || System.Enum.Parse(typeof(WeatherType), worstweather) == null)
            {
                TempData["Error-Message"] = "Error! This weather type does not exist. For example: sunny or rain";
                return View("ActivityDashBoard");
            }

            WeatherType _bestWeather = (WeatherType) System.Enum.Parse(typeof(WeatherType), bestweather);
            WeatherType _worstWeather = (WeatherType) System.Enum.Parse(typeof(WeatherType), worstweather);

            if (_bestWeather == _worstWeather)
            {
                TempData["Error-Message"] = "Error! Best weather and worst weather are identical value";
                return View("ActivityDashBoard");
            }*/

            var english = CultureInfo.GetCultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = english;

            var _lat = Double.Parse(lat);
            var _lon = Double.Parse(lon);

            if ((_lat < -90 || _lat > 90) || (_lon < -180 || _lon > 180))
            {
                TempData["Error-Message"] = "Invaild coordinate input!";
                return View("ActivityDashBoard");
            }

            var activity = new Models.Activities.Activity();
 
            activity.Name = name;
            activity.City = city;
            activity.Image_Path = path.Replace("./wwwroot", "");
            activity.Coordinate = new Coordinate(_lat, _lon);
            activity.WeatherTypes = new WeatherType[2];
            activity.WeatherTypes[0] = WeatherType.SUNNY; //_bestWeather
            activity.WeatherTypes[1] = WeatherType.RAIN; //_worstWeather

            using (Stream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }

            if (Models.Activities.Activity.activities is not null)
            {
                Models.Activities.Activity.activities.Add(activity);
                var json = JsonConvert.SerializeObject(Models.Activities.Activity.activities, Formatting.Indented);
                System.IO.File.WriteAllText("./activity.json", json);
                TempData["Successful"] = name + " is now added as a new activity!";
            }

            return View("ActivityDashBoard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}