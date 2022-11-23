using Newtonsoft.Json;
using System.Diagnostics;

namespace activityfinder_asp.net.Models.Dto
{
    public class Account
    {

        public static void ParseDataBase()
        {
            DirectoryInfo directory = new DirectoryInfo("./data/");
            foreach (FileInfo file in directory.GetFiles())
            {
                using (StreamReader reader = File.OpenText(file.FullName))
                {
                    if (!file.Name.EndsWith(".json"))
                    {
                        Debug.WriteLine("Skipping: " + file.FullName + ". [Reason: NOT JSON!]");
                        continue;
                    }
                    string json = reader.ReadToEnd();
                    Account account = JsonConvert.DeserializeObject<Account>(json);
                    if (account is not null)
                    {
                       Constant.accounts.Add(account);
                    }
                }
            }
            Debug.WriteLine(Constant.accounts.Count + " accounts loaded into the Account Database.");
        }

        public long Id = Convert.ToInt64(Guid.NewGuid().GetHashCode().ToString().Replace("-", ""));

        public string Name { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string RepeatEmail { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public string RepeatPassword { get; set; }

        public bool Verified { get; set; }

    }
}
