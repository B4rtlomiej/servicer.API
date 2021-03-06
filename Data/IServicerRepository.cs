using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using servicer.API.Helpers;
using servicer.API.Models;

namespace servicer.API.Data
{
    public interface IServicerRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
        Task<User> GetUser(string username);
        Task ChangeIsActive(User user);
        Task<PagedList<Ticket>> GetTickets(TicketParams ticketParams);
        Task<Ticket> GetTicket(int id);
        Task<Ticket> CreateTicket(Ticket ticket);
        Task DeleteTicket(int id);
        Task<PagedList<ProductSpecification>> GetProductSpecifications(ProductSpecificationParams productParams);
        Task<ProductSpecification> GetProductSpecification(int id);
        Task<int?> GetProductSpecificationId(string manufacturer, string series, string name);
        Task<ProductSpecification> CreateProductSpecification(ProductSpecification productSpecification);
        Task<int?> GetCustomerId(string email, string firstName, string lastName);
        Task<int?> GetItemId(int productSpecificationId, int customerId, DateTime productionYear);
        Task<IEnumerable<Note>> GetNotes();
        Task<Note> GetNote(int id);
        Task<Note> CreateNote(Note note);
        Task<IEnumerable<Note>> GetTicketNotes(int ticketId);
        Task<IEnumerable<Note>> GetCustomerNotes(int customerId);
        Task<IEnumerable<Note>> GetItemNotes(int itemId);
        Task<IEnumerable<Note>> GetProductSpecificationNotes(int productSpecificationId);
        Task DeleteNote(int id);
        Task CloseTicket(Ticket ticket);
        Task<PagedList<Person>> GetPersons(PersonParams personParams);
        Task<Person> GetPerson(int id);
        int GetCustomerIdByPersonId(int personId);
        string GetAgentsWithMostClosedTickets();
        string GetProductsWithMostTickets();
        string GetTicketsByMonths();
        string GetClosedTicketsByMonths();
    }
}