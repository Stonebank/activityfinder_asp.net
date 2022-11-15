using activityfinder_asp.net.Models;
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

        public IActionResult Discover()
        {
            return View();
        }

        [HttpPost]
        public void Test(string coords)
        {
            Debug.WriteLine(coords);
            double lat = Convert.ToDouble(coords.Split(" ")[0].Replace(".", ","));
            double lon = Convert.ToDouble(coords.Split(" ")[1].Replace(".", ","));
            Debug.WriteLine(lat + " " + lon);
            UserLocation userLocation = new UserLocation(new Coordinate(lat, lon));
            Debug.WriteLine(userLocation.Coordinate.Latitude + " " + userLocation.Coordinate.Longitude);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}