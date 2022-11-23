using activityfinder_asp.net.Models.Location;

namespace activityfinder_asp.net.Models.Activities.Category
{
    public abstract class Category
    {

        public abstract void Compare(UserLocation userLocation);

        public abstract void AddPoints();

        public abstract Activity BestCandidate();

    }
}
