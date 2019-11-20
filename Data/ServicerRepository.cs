using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            if (!string.IsNullOrEmpty(userParams.orderBy) && userParams.orderBy == "lastActive")
                users = users.OrderByDescending(u => u.LastActive);
            if (!string.IsNullOrEmpty(userParams.orderBy) && userParams.orderBy == "lastCreated")
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
            var tickets = _context.Tickets
                                    .Include(t => t.Item)
                                    .Include(ps => ps.Item.ProductSpecification)
                                    .Include(c => c.Item.Customer)
                                    .Include(u => u.User ).AsQueryable();
            
            if (ticketParams.UserId != null)
                tickets = tickets.Where(u => u.UserId == Convert.ToInt32(ticketParams.UserId));                       

            tickets = tickets.Where(t => t.Priority == ticketParams.priority);
            tickets = tickets.Where(t => t.Status == ticketParams.status);
            if (!string.IsNullOrEmpty(ticketParams.orderBy) && ticketParams.orderBy == "lastOpen")
                tickets = tickets.OrderByDescending(u => u.Created);
            if (!string.IsNullOrEmpty(ticketParams.orderBy) && ticketParams.orderBy == "lastClosed")
                tickets = tickets.OrderByDescending(u => u.Closed);

            return await PagedList<Ticket>.CreateAsync(tickets, ticketParams.PageNumber, ticketParams.PageSize);
        }

        public async Task<Ticket> GetTicket(int id)
        {
            var ticket = await _context.Tickets.Include(t => t.Item).Include(ps => ps.Item.ProductSpecification).Include(c => c.Item.Customer)
            .Include(per => per.Item.Customer.Person).FirstOrDefaultAsync(t => t.Id == id);

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

        public async Task<PagedList<ProductSpecification>> GetProductSpecifications(ProductSpecificationParams productParams)
        {
            var productSpecifications = _context.ProductSpecifications.OrderBy(p =>p.Name).AsQueryable();

            if (!string.IsNullOrEmpty(productParams.isActive))
            {
                if (productParams.isActive == "active")                
                    productSpecifications = productSpecifications.Where(p => p.IsActive == true);
                else
                    productSpecifications = productSpecifications.Where(p => p.IsActive == false);
            }
            

            if(!string.IsNullOrEmpty(productParams.column) && productParams.column == "name" && !string.IsNullOrEmpty(productParams.sorting))
            {
                if(productParams.sorting =="ascending")
                    productSpecifications = productSpecifications.OrderBy(p =>p.Name);
                else
                    productSpecifications = productSpecifications.OrderByDescending(p =>p.Name);
            }

            if(!string.IsNullOrEmpty(productParams.column) && productParams.column == "series" && !string.IsNullOrEmpty(productParams.sorting))
            {
                if(productParams.sorting =="ascending")
                    productSpecifications = productSpecifications.OrderBy(p =>p.Series);
                else
                    productSpecifications = productSpecifications.OrderByDescending(p =>p.Series);
            }

            if(!string.IsNullOrEmpty(productParams.column) && productParams.column == "manufacturer" && !string.IsNullOrEmpty(productParams.sorting))
            {
                if(productParams.sorting =="ascending")
                    productSpecifications = productSpecifications.OrderBy(p =>p.Manufacturer);
                else
                    productSpecifications = productSpecifications.OrderByDescending(p =>p.Manufacturer);
            }

            return await PagedList<ProductSpecification>.CreateAsync(productSpecifications, productParams.PageNumber, productParams.PageSize);
        }

        public async Task<ProductSpecification> GetProductSpecification(int id)
        {
            var productSpecification = await _context.ProductSpecifications.FirstOrDefaultAsync(pS => pS.Id == id);

            return productSpecification;
        }

        public async Task<int?> GetProductSpecificationId(string manufacturer, string series, string name)
        {
            var productSpecification = await _context.ProductSpecifications.FirstOrDefaultAsync(pS =>
                pS.Manufacturer == manufacturer && pS.Series == series && pS.Name == name && pS.IsActive
            );

            int? id;
            if (productSpecification != null)
            {
                id = productSpecification.Id;
            }
            else
            {
                id = null;
            }

            return id;
        }

        public async Task<ProductSpecification> CreateProductSpecification(ProductSpecification productSpecification)
        {
            productSpecification.IsActive = true;
            await _context.ProductSpecifications.AddAsync(productSpecification);
            await _context.SaveChangesAsync();

            return productSpecification;
        }

        public async Task<int?> GetCustomerId(string email, string firstName, string lastName)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c =>
                c.Person.Email == email && c.Person.FirstName == firstName && c.Person.LastName == lastName
            );

            int? id;
            if (customer != null)
            {
                id = customer.Id;
            }
            else
            {
                id = null;
            }

            return id;
        }

        public async Task<int?> GetItemId(int productSpecificationId, int customerId, DateTime productionYear)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i =>
                i.ProductSpecificationId == productSpecificationId && i.CustomerId == customerId && i.ProductionYear == productionYear
            );

            int? id;
            if (item != null)
            {
                id = item.Id;
            }
            else
            {
                id = null;
            }

            return id;
        }

        public async Task<IEnumerable<Note>> GetNotes()
        {
            var notes = await _context.Notes.ToListAsync();

            return notes;
        }

        public async Task<Note> GetNote(int id)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);

            return note;
        }

        public async Task<Note> CreateNote(Note note)
        {
            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();

            return note;
        }
        public async Task ChangeOwnerTicket(Ticket ticket, int UserId)
        {
            ticket.UserId = UserId;
            await _context.SaveChangesAsync();
        }
        public async Task SetStatus(Ticket ticket)
        {
            ticket.Status = (Status)1;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetEmailAddressByItemId(int id)
        {
            var customerId =  _context.Items.FirstOrDefault(i =>i.Id == id).CustomerId;
            var emailAddress =  _context.People.FirstOrDefault(c => c.CustomerId == customerId).Email;
            return emailAddress;
        }

        public async Task CloseTicket(Ticket ticket)
        {
            ticket.Status = (Status)2;
            ticket.Closed = DateTime.Today.ToLocalTime();
            await _context.SaveChangesAsync();
        }
    }
}