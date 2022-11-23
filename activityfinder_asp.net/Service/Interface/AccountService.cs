using activityfinder_asp.net.Models.Dto;
using Newtonsoft.Json;

namespace activityfinder_asp.net.Service.Interface
{
    public interface AccountService
    {

        void Save(Account account);
        void SendVerificationEmail(Account account, string token);

        Account Load(string email);
        Account Load(long id);

    }
}
