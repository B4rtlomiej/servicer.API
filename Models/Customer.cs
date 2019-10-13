using System.Collections.Generic;

namespace servicer.API.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public Person Person { get; set; }

        public SupportLevel SupportLevel { get; set; }

        public ICollection<Note> Notes { get; set; }

        public ICollection<Item> Items { get; set; }

        public Customer()
        {
        }
    }
}