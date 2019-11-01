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
        Task<User> GetUser(string username);
        Task<User> GetInactiveUser(string username);
        Task<IEnumerable<Ticket>> GetTickets();
        Task<Ticket> GetTicket(int id);
        Task<Ticket> CreateTicket(Ticket ticket);
        Task DeleteTicket(int id);
    }
}