using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Person).OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.UserRole == userParams.UserRole);
            users = users.Where(u => u.IsActive == userParams.IsActive);

            if (!string.IsNullOrEmpty(userParams.OrderBy) && userParams.OrderBy == "lastActive")
                users = users.OrderByDescending(u => u.LastActive);
            if (!string.IsNullOrEmpty(userParams.OrderBy) && userParams.OrderBy == "lastCreated")
                users = users.OrderByDescending(u => u.Created);

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

         public async Task<PagedList<Person>> GetPersons(PersonParams personParams)
        {
            var persons = _context.People.Where(p => p.CustomerId != null).AsQueryable();
            if(!string.IsNullOrEmpty(personParams.Column)) {
                if(personParams.Column == "firstName" && !string.IsNullOrEmpty(personParams.Sorting))
                {
                    if(personParams.Sorting =="ascending")
                        persons = persons.OrderBy(p =>p.FirstName);
                    else
                        persons = persons.OrderByDescending(p =>p.FirstName);
                }

                if(personParams.Column == "lastName" && !string.IsNullOrEmpty(personParams.Sorting))
                {
                    if(personParams.Sorting =="ascending")
                        persons = persons.OrderBy(p =>p.LastName);
                    else
                        persons = persons.OrderByDescending(p =>p.LastName);
                }

                if(personParams.Column == "email" && !string.IsNullOrEmpty(personParams.Sorting))
                {
                    if(personParams.Sorting =="ascending")
                        persons = persons.OrderBy(p =>p.Email);
                    else
                        persons = persons.OrderByDescending(p =>p.Email);
            }
            }

            return await PagedList<Person>.CreateAsync(persons, personParams.PageNumber, personParams.PageSize);
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
                                    .Include(u => u.User).AsQueryable();
            
            if (ticketParams.PersonId != 0)
            {
                var customers = _context.Customers.Include(c => c.Person);
                var customer = customers.FirstOrDefault(c => c.Person.Id == ticketParams.PersonId);
                if(customer !=null)
                  tickets = tickets.Where(t => t.Item.CustomerId == customer.Id);
            }
            else 
            {
            if (!string.IsNullOrEmpty(ticketParams.Token))
                tickets = tickets.Where(t => t.UserId == Convert.ToInt32(ticketParams.Token));                       
           
            if (!string.IsNullOrEmpty(ticketParams.OrderBy) && ticketParams.OrderBy == "lastOpen")
                tickets = tickets.OrderByDescending(u => u.Created);
            if (!string.IsNullOrEmpty(ticketParams.OrderBy) && ticketParams.OrderBy == "lastClosed")
                tickets = tickets.OrderByDescending(u => u.Closed);

            tickets = tickets.Where(t => t.Priority == ticketParams.Priority);
            tickets = tickets.Where(t => t.Status == ticketParams.Status);
            }

            return await PagedList<Ticket>.CreateAsync(tickets, ticketParams.PageNumber, ticketParams.PageSize);
        }

        public int GetCustomerIdByPersonId(int personId)
        {
            var customers = _context.Customers.Include(c => c.Person);
            var customer = customers.FirstOrDefault(c => c.Person.Id == personId);

            return customer.Id;
        }

        public async Task<Ticket> GetTicket(int id)
        {
            var ticket = await _context.Tickets.Include(t => t.Item).Include(ps => ps.Item.ProductSpecification).Include(c => c.Item.Customer)
            .Include(per => per.Item.Customer.Person).Include(u => u.User).Include(up => up.User.Person).FirstOrDefaultAsync(t => t.Id == id);

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

            if (!string.IsNullOrEmpty(productParams.IsActive))
            {
                if (productParams.IsActive == "active")
                    productSpecifications = productSpecifications.Where(p => p.IsActive == true);
                else
                    productSpecifications = productSpecifications.Where(p => p.IsActive == false);
            }

            if(!string.IsNullOrEmpty(productParams.Column) && productParams.Column == "name" && !string.IsNullOrEmpty(productParams.Sorting))
            {
                if(productParams.Sorting =="ascending")
                    productSpecifications = productSpecifications.OrderBy(p =>p.Name);
                else
                    productSpecifications = productSpecifications.OrderByDescending(p =>p.Name);
            }

            if(!string.IsNullOrEmpty(productParams.Column) && productParams.Column == "series" && !string.IsNullOrEmpty(productParams.Sorting))
            {
                if(productParams.Sorting =="ascending")
                    productSpecifications = productSpecifications.OrderBy(p =>p.Series);
                else
                    productSpecifications = productSpecifications.OrderByDescending(p =>p.Series);
            }

            if(!string.IsNullOrEmpty(productParams.Column) && productParams.Column == "manufacturer" && !string.IsNullOrEmpty(productParams.Sorting))
            {
                if(productParams.Sorting =="ascending")
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

        public async Task<IEnumerable<Note>> GetTicketNotes(int ticketId)
        {
            var notes = await _context.Notes.Where(n => n.TicketId == ticketId).ToListAsync();

            return notes;
        }

        public async Task<IEnumerable<Note>> GetCustomerNotes(int customerId)
        {
            var notes = await _context.Notes.Where(n => n.CustomerId == customerId).ToListAsync();

            return notes;
        }

        public async Task<IEnumerable<Note>> GetItemNotes(int itemId)
        {
            var notes = await _context.Notes.Where(n => n.ItemId == itemId).ToListAsync();

            return notes;
        }

        public async Task<IEnumerable<Note>> GetProductSpecificationNotes(int productSpecificationId)
        {
            var notes = await _context.Notes.Where(n => n.ProductSpecificationId == productSpecificationId).ToListAsync();

            return notes;
        }

        public async Task DeleteNote(int id)
        {
            var noteToRemove = await GetNote(id);
            _context.Notes.Remove(noteToRemove);
        }

        public async Task CloseTicket(Ticket ticket)
        {
            ticket.Status = Status.Closed;
            ticket.Closed = DateTime.Now;
            await _context.SaveChangesAsync();
        }

       public async Task<Person> GetPerson(int id)
        {
            var ticket = await _context.People.FirstOrDefaultAsync(p => p.Id == id);

            return ticket;
        }

        public string GetAgentsWithMostClosedTickets()
        {
            var queryLastNames =
                (from ticket in _context.Tickets
                where ticket.UserId != null
                group ticket by new {ticket.UserId, ticket.User.Person.FirstName, ticket.User.Person.LastName} into newGroup
                orderby newGroup.Count() descending
                select new
                {
                    UserId = newGroup.Key.UserId,
                    FirstName = newGroup.Key.FirstName,
                    LastName = newGroup.Key.LastName,
                    TicketCount = newGroup.Count()
                }).Take(5);

           return JsonConvert.SerializeObject(queryLastNames);
        }

        public string GetProductsWithMostTickets()
        {
            var queryProductsWithMostTickets =
                (from ticket in _context.Tickets
                group ticket by new {ticket.Item.ProductSpecificationId, ticket.Item.ProductSpecification.Manufacturer,
                    ticket.Item.ProductSpecification.Series, ticket.Item.ProductSpecification.Name} into newGroup
                orderby newGroup.Count() descending
                select new
                {
                    ProductSpecificationId = newGroup.Key.ProductSpecificationId,
                    Manufacturer = newGroup.Key.Manufacturer,
                    Series = newGroup.Key.Series,
                    Name = newGroup.Key.Name,
                    TicketCount = newGroup.Count()
                }).Take(5);

           return JsonConvert.SerializeObject(queryProductsWithMostTickets);
        }
    }
}