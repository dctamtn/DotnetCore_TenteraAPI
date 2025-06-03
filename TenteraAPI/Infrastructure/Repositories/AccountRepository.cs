using Microsoft.EntityFrameworkCore;
using TenteraAPI.Domain.Entities;
using TenteraAPI.Domain.Interfaces.Repositories;
using TenteraAPI.Infrastructure.Database;

namespace TenteraAPI.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDbContext _context;

        public AccountRepository(AccountDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Account customer)
        {
            await _context.Accounts.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Account> GetICNumberAsync(string iCnumber)
        {
            return await _context.Accounts.FirstOrDefaultAsync(c => c.ICNumber == iCnumber);
        }
        public async Task<bool> ICNumberExistsAsync(string iCnumber)
        {
            return await _context.Accounts.AnyAsync(c => c.ICNumber == iCnumber);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Accounts.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await _context.Accounts.AnyAsync(c => c.PhoneNumber == phoneNumber);
        }

        public async Task UpdateAsync(Account customer)
        {
            _context.Accounts.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}
