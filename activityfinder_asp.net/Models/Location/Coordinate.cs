namespace activityfinder_asp.net.Models.Location
{

    public class Coordinate {

        public double Latitude { get; set;  }
        public double Longitude { get; set; }

        public Coordinate(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

    }

}