namespace activityfinder_asp.net.Models.Location
{

    public class Coordinate {

        private double Latitude { get; set;  }
        private double Longitude { get; set; }

        public Coordinate(double latititude, double longitude)
        {
            this.Latitude = latititude;
            this.Longitude = longitude;
        }

    }

}