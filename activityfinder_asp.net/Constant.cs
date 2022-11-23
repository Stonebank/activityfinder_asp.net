using activityfinder_asp.net.Models.Dto;
using System.Text.RegularExpressions;

namespace activityfinder_asp.net
{
    public class Constant
    {

        public static bool DEBUG = true;

        public static string EMAIL = "activityjava@gmail.com";
        public static string PASSWORD = "vdvrdvofdiewbgmk";

        public static string WEATHER_API_KEY = "0019c2b6a2cfe068caaf41a3e8ffb3cd";
        public static string WEATHER_UNIT_OUTPUT = "metric";

        public static string PASSWORD_SALT = "D60845F496EEC2251CD39FD9B5872625";

        public static List<Account> accounts = new List<Account>();

        public static bool HasPasswordRequirement(string password)
        {
            var match = Regex.Match(password, "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\\S+$).{8,}$");
            return match.Success;
        }

    }
}
