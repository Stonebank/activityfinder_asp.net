using activityfinder_asp.net.Models.Location;
using System.Collections;

namespace activityfinder_asp.net.Models.Activities
{

    public class Activity
    {

        public static List<Activity>? activities;

        public Coordinate Coordinate { get; set; }

        public string Name { get; set; }
        public string City { get; set; }

        public string Image_Path { get; set; }

        public int Points { get; set; }

        public int[] Ratings { get; set; }

        public bool IsFavorite { get; set; }

        public string Link()
        {
            string lat = Coordinate.Latitude.ToString().Replace(",", ".");
            string lon = Coordinate.Longitude.ToString().Replace(",", ".");
            return "http://www.google.com/maps/place/" + lat + "," + lon;
        }

 
    }


}