using activityfinder_asp.net.Models;
using activityfinder_asp.net.Models.Activities.Category;
using activityfinder_asp.net.Models.Activities.Category.Container;
using activityfinder_asp.net.Models.Location;
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

        public IActionResult Discover(string coords)
        {
            var lat = 1.1;
            var lon = 1.1;
            if (coords != null)
            {
                lat = Convert.ToDouble(coords.Split(" ")[0].Replace(".", ","));
                lon = Convert.ToDouble(coords.Split(" ")[1].Replace(".", ","));
                Debug.WriteLine("After converting: " + lat + " " + lon);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}