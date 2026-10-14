using System.Threading.Tasks;

namespace aLeeTheY.Utils.Services
{
    public interface IAuthenticationService
    {
        Task<bool> Register(string username, string password);
    }
}
