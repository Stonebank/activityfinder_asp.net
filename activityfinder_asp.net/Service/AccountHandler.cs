using activityfinder_asp.net.Models.Dto;
using activityfinder_asp.net.Security;
using activityfinder_asp.net.Service.Interface;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace activityfinder_asp.net.Service
{
    public class AccountHandler : AccountService
    {
        public Account Load(string email)
        {
            if (!Directory.Exists("./data/"))
            {
                throw new DirectoryNotFoundException("Error! Directory data does not exist. Create one in main folder");
            }
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
            if (!Directory.Exists("./data/"))
            {
                throw new DirectoryNotFoundException("Error! Directory data does not exist. Create one in main folder");
            }
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
            if (!Constant.accounts.Contains(account))
                Constant.accounts.Add(account);
        }

        public void SendVerificationEmail(Account account, string host, string token)
        {
            string link = "https://" + host + "/home/register/confirmation?token=" + token;
            Email.Send(account.Email, "Confirm your registration", "Hello " + account.Name + "!<br><br>Thank you for registering. Click the link below to complete your registration<br>" + link);
        }

        public void SendRecoveryEmail(Account account, string host, string token)
        {
            string link = "https://" + host + "/home/requestchangepassword/request?token=" + token;
            Email.Send(account.Email, "Reset your password", "Hi " + account.Name + ",<br><br>Someone recently requested a password change for your account. If this was you, you can set a new password here:<br>" + link + "<br><br>If you didn't request this password change, we strongly suggest to change your password as soon as possible.");
        }

        public void SendEmail(Account account, string topic, string body)
        {
            Email.Send(account.Email, topic, body);
        }

    }
}
