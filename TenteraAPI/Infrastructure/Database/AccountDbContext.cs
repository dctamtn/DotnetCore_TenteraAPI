using System.Collections.Generic;
using TenteraAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TenteraAPI.Infrastructure.Database
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}
