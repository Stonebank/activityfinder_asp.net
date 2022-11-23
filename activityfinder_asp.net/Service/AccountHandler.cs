using activityfinder_asp.net.Models.Dto;
using activityfinder_asp.net.Service.Interface;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json;

namespace activityfinder_asp.net.Service
{
    public class AccountHandler : AccountService
    {
        public Account Load(string email)
        {
            if (Constant.accounts is null)
            {
                throw new Exception("Error! Account DB is null.");
            }
            for (int i = 0; i < Constant.accounts.Count; i++)
            {
                if (Constant.accounts[i] == null || !Constant.accounts[i].Email.ToLower().Equals(email.ToLower()))
                    continue;
                return Constant.accounts[i];
            }
            return null;
        }

        public Account Load(long id)
        {
            using (StreamReader reader = File.OpenText("./data/" + id + ".json"))
            {
                string json = reader.ReadToEnd();
                Account account = JsonConvert.DeserializeObject<Account>(json);
                if (account is null)
                {
                    Debug.WriteLine("Error! Account with id: " + id + " does not exist.");
                    return null;
                }
                return account;
            }
        }

        public void Save(Account account)
        {
            var json = JsonConvert.SerializeObject(account, Formatting.Indented);
            File.WriteAllText("./data/" + account.Id + ".json", json);
            Debug.WriteLine("[JSON DATA WRITTEN FOR " + account.Id + "]");
            // TODO: Add in the static array
        }

        public void SendVerificationEmail(Account account, string token)
        {
            
        }

    }
}
