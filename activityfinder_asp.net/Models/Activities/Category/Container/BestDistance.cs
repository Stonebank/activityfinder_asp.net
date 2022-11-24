using activityfinder_asp.net.Models.Location;

namespace activityfinder_asp.net.Models.Activities.Category.Container
{
    public class BestDistance : Category
    {

        private Activity[]? activities;

        public override void Compare(UserLocation userLocation)
        {

            if (Activity.activities is null)
            {
                throw new Exception("Error! Comparing algorithm could not be performed.");
            }

            activities = new Activity[Activity.activities.Count];

            for (int i = 0; i< activities.Length; i++)
            {
                activities[i] = Activity.activities[i];
            }

            for (int i = 0; i < activities.Length; i++)
            {
                double distance_i = userLocation.CalculateDistance(activities[i].Coordinate);
                for (int j = 0; j < activities.Length; j++)
                {
                    double distance_j = userLocation.CalculateDistance(activities[j].Coordinate);
                    if (distance_i == distance_j)
                        continue;
                    if (distance_i < distance_j)
                    {
                        var temp = activities[i];
                        activities[i] = activities[j];
                        activities[j] = temp;
                    }
                }
            }

        }

        public override void AddPoints()
        {
            if (activities is null)
            {
                throw new Exception("Error! Comparison algorithm could not be performed.");
            }
            for (int i = 0; i < activities.Length; i++)
            {
                int points = (1000 * activities.Length) / (i + 1);
                Activity activity = Activity.GetActivity(activities[i].Name) ?? activities[0];
                if (activity == null)
                {
                    throw new Exception("Error! Could not add points at index " + i);
                }
                activity.Points += points;
            }
        }

        public override Activity BestCandidate()
        {
            if (activities is null)
            {
                throw new Exception("Best Candidtate could not be assigned.");
            }
            return activities[0];
        }


    }
}
