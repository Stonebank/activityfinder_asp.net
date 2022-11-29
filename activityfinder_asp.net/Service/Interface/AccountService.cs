using activityfinder_asp.net.Models.Dto;
using Newtonsoft.Json;

namespace activityfinder_asp.net.Service.Interface
{
    public interface AccountService
    {

        void Save(Account account);
        void SendVerificationEmail(Account account, string host, string token);
        void SendRecoveryEmail(Account account, string host, string token);
        void SendEmail(Account account, string topic, string body);

        Account Load(string email);
        Account Load(long id);

    }
}
