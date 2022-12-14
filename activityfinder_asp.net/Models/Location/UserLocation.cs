using activityfinder_asp.net.Models.Activities;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using Activity = activityfinder_asp.net.Models.Activities.Activity;

namespace activityfinder_asp.net.Models.Location
{
    public class UserLocation
    {

        private JObject jsonObject;

        public Coordinate Coordinate { get; set; }

        public double current_temperature { get; set; }
        public double maximum_temperature { get; set; }
        public double minimum_temperature { get; set; }
        public double feels_like { get; set; }

        public WeatherType WeatherType { get; set; }

        public UserLocation(Coordinate coordinate)
        {
            this.Coordinate = coordinate;
        }

        public async void FetchWeatherData()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync("https://api.openweathermap.org/data/2.5/weather?lat=" + Coordinate.Latitude + "&lon=" + Coordinate.Longitude + "&appid=" + Constant.WEATHER_API_KEY + "&units=" + Constant.WEATHER_UNIT_OUTPUT);
                jsonObject = JObject.Parse(json);
                ParseWeatherData();
            }
        }

        private void ParseWeatherData()
        {
            if (jsonObject is null || (Coordinate.Latitude == 0 && Coordinate.Longitude == 0))
            {
                throw new Exception("Error! Weather data could not be parsed.");
            }
            var main = jsonObject["main"];
            var weather = jsonObject["weather"];
            if (main is null || weather is null)
            {
                throw new Exception("Error! Something went wrong while parsing the Weather Data...");
            }
            current_temperature = Math.Round(main["temp"].Value<double>());
            maximum_temperature = Math.Round(main["temp_max"].Value<double>());
            minimum_temperature = Math.Round(main["temp_min"].Value<double>());
            feels_like = Math.Round(main["feels_like"].Value<double>());

            string currentWeather = weather[0]["description"].Value<string>();

            switch (currentWeather.ToLower())
            {

                case "clear sky":
                case "sunny":
                case "sun":
                    WeatherType = WeatherType.SUNNY;
                    break;
                case "overcast clouds":
                case "few clouds":
                case "scattered clouds":
                case "broken clouds":
                    WeatherType = WeatherType.CLOUD;
                    break;
                case "shower rain":
                case "rain":
                case "mist":
                case "drizzle":
                    WeatherType = WeatherType.RAIN;
                    break;
                case "thunderstorm":
                case "tornado":
                    WeatherType = WeatherType.STORM;
                    break;
                case "snow":
                    WeatherType = WeatherType.SNOW;
                    break;
                case "wind":
                case "windy":
                    WeatherType = WeatherType.WIND;
                    break;
                case "none":
                    WeatherType = WeatherType.NONE;
                    break;
                default:
                    Debug.WriteLine("WeatherType not detected");
                    break;
                 
            }

        }

        public string GetDistance(Activity activity)
        {
            return Math.Round(CalculateDistance(activity.Coordinate)) + " km";
        }

        public double CalculateDistance(Coordinate toCoordinate)
        {
            if (this.Coordinate == null)
            {
                throw new ArgumentNullException("Error! Argument [coordinate] is null.");
            }

            if (this.Coordinate.Latitude == 0 || toCoordinate.Latitude == 0 || this.Coordinate.Longitude == 0 || toCoordinate.Longitude == 0)
            {
                throw new ArgumentException("One of the assigned coordinates is 0.");
            }

            if (this.Coordinate == toCoordinate)
            {
                throw new ArgumentException("The input coordinate is the same as instance coordinate");
            }

            int earthRadius = 6371;

            double lat1 = Math.Min(this.Coordinate.Latitude, toCoordinate.Latitude);
            double lat2 = Math.Max(this.Coordinate.Latitude, toCoordinate.Latitude);

            double lon1 = Math.Min(this.Coordinate.Longitude, toCoordinate.Longitude);
            double lon2 = Math.Max(this.Coordinate.Longitude, toCoordinate.Longitude);

            double deltaLat = ToRadians(lat2 - lat1);
            double deltaLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2)
                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));


            return earthRadius * c;

        }

        private double ToRadians(double angle)
        {
            if (angle == 0)
            {
                throw new ArgumentException("Error! Angle must be != 0");
            }
            return (Math.PI / 180) * angle;
        }

    }
}
