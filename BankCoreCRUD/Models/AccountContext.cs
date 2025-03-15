using Microsoft.EntityFrameworkCore;

namespace BankCoreCRUD.Models
{
    public class AccountContext :DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options) : base(options) 
        { 
        
        }
        public DbSet<Account> Account { get; set; } = default;
        public DbSet<Customer> Customer { get; set; } = default;
        public DbSet<Transaction> Transaction { get; set; } = default;
    }
}
