using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servicer.API.Helpers;
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
            var user = await _context.Users.Include(p => p.Person).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<User> GetUser(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<User> GetInactiveUser(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username && !x.IsActive);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Person).OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.UserRole == userParams.userRole);
            users = users.Where(u => u.IsActive == userParams.isActive);

            if (userParams.orderBy == "lastActive")
                users = users.OrderByDescending(u => u.LastActive);
            if (userParams.orderBy == "lastCreated")
                users = users.OrderByDescending(u => u.Created);

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task ChangeIsActive(User user)
        {
            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task<PagedList<Ticket>> GetTickets(TicketParams ticketParams)
        {
            var tickets = _context.Tickets.Include(t => t.Item).Include(c => c.Item.Customer).AsQueryable();

            tickets = tickets.Where(t => t.Priority == ticketParams.priority);
            tickets = tickets.Where(t => t.Status == ticketParams.status);
            if (ticketParams.orderBy == "lastOpen")
                tickets = tickets.OrderByDescending(u => u.Created);
            if (ticketParams.orderBy == "lastClosed")
                tickets = tickets.OrderByDescending(u => u.Closed);

            return await PagedList<Ticket>.CreateAsync(tickets, ticketParams.PageNumber, ticketParams.PageSize);
        }

        public async Task<Ticket> GetTicket(int id)
        {
            var ticket = await _context.Tickets.Include(t => t.Item).Include(c => c.Item.Customer).FirstOrDefaultAsync(t => t.Id == id);

            return ticket;
        }

        public async Task<Ticket> CreateTicket(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task DeleteTicket(int id)
        {
            var ticketToRemove = await GetTicket(id);
            _context.Tickets.Remove(ticketToRemove);
        }
    }
}