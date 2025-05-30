using Microsoft.EntityFrameworkCore;
using userscan.Models; 

namespace userscan.Data
{
    public class ContactsDbContext : DbContext
    {
        public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; }
    }
}
