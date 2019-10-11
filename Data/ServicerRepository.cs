using System.Collections.Generic;
using System.Threading.Tasks;
using servicer.API.Models;

namespace servicer.API.Data
{
    public class ServicerRepository : IServicerRepository
    {
        private readonly DataContext _context;
        public ServicerRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            // TODO: retrieve
            //var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(user => user.Id == id);

            return null;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            // TODO: retrieve
            //var users = await _context.Users.Include(p => p.Photos).ToListAsync();

            return null;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}