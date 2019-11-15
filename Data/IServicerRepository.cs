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
        Task<User> GetInactiveUser(string username);
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
    }
}