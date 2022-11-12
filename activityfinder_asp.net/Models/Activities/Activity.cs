using activityfinder_asp.net.Models.Location;
using System.Collections;

namespace activityfinder_asp.net.Models.Activities
{

    public class Activity
    {

        public static ArrayList activities = new ArrayList();

        private Coordinate Coordinate { get; set; }

        private string Name { get; set; }
        private string City { get; set; }

        private string Link { get; set; }
        private string Image_Path { get; set; }

        private int Points { get; set; }

        private int[] Ratings { get; set; }

        private bool IsFavorite { get; set; }


    }

}