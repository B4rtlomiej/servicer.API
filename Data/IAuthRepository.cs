using System.Threading.Tasks;
using servicer.API.Models;

namespace servicer.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<User> SetPassword(User user, string password);
    }
}