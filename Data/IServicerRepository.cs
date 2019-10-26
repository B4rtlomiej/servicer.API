using System.Collections.Generic;
using System.Threading.Tasks;
using servicer.API.Models;

namespace servicer.API.Data
{
    public interface IServicerRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task<User> GetInactiveUser(string username);
    }
}