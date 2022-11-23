using activityfinder_asp.net.Models.Location;

namespace activityfinder_asp.net.Models.Activities.Category.Container
{
    public class BestWeather : Category
    {
        public override void Compare(UserLocation userLocation)
        {
            if (Activity.activities is null)
            {
                throw new NullReferenceException("Error! Activities not initialized.");
            }

            userLocation.ParseWeatherData();

            for (int i = 0; i < Activity.activities.Count; i++)
            {
               if (Activity.activities[i].GetBestWeather() == userLocation.WeatherType || Activity.activities[i].GetBestWeather() == WeatherType.NONE)
                {
                    Activity.activities[i].Points++;
                }
               if (Activity.activities[i].GetWorstWeather() == userLocation.WeatherType)
                {
                    Activity.activities[i].Points--;
                }
            }

        }

        public override void AddPoints()
        {
            
        }

        public override Activity BestCandidate()
        {
            return null;
        }

    }
}
