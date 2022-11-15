namespace activityfinder_asp.net.Models.Location
{

    public class Coordinate {

        public double Latitude { get; set;  }
        public double Longitude { get; set; }

        public Coordinate(double latititude, double longitude)
        {
            this.Latitude = latititude;
            this.Longitude = longitude;
        }

    }

}