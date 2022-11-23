using activityfinder_asp.net.Models.Location;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;

namespace activityfinder_asp.net.Models.Activities
{

    public class Activity
    {

        public static List<Activity>? activities;

        public Coordinate Coordinate { get; set; }

        public WeatherType[] WeatherTypes { get; set; }

        public string Name { get; set; }
        public string City { get; set; }

        public string Image_Path { get; set; }

        public int Points { get; set; }

        public int[] Ratings { get; set; }

        public bool IsFavorite { get; set; }

        public WeatherType GetBestWeather()
        {
            return WeatherTypes[0];
        }

        public WeatherType GetWorstWeather()
        {
            if (WeatherTypes.Length > 0)
            {
                return WeatherTypes[WeatherTypes.Length - 1];
            }
            return WeatherType.NONE;
        }

        public static Activity? GetActivity(string name)
        {
            if (activities is null)
            {
                throw new ArgumentNullException("Activities array is null.");
            }
            for (int i = 0; i < activities.Count; i++)
            {
                if (activities[i].Name.ToLower().Equals(name.ToLower()))
                    return activities[i];
            }
            return null;
        }

        public static void ParseAndLoadJson()
        {
            using (StreamReader reader = File.OpenText("activity.json"))
            {
                string json = reader.ReadToEnd();
                activities = JsonConvert.DeserializeObject<List<Activity>>(json);
            }
            if (activities != null)
                Debug.WriteLine("[Activity class] Initialized " + activities.Count + " activities");
        }

        public string Link()
        {
            string lat = Coordinate.Latitude.ToString().Replace(",", ".");
            string lon = Coordinate.Longitude.ToString().Replace(",", ".");
            return "http://www.google.com/maps/place/" + lat + "," + lon;
        }

 
    }


}