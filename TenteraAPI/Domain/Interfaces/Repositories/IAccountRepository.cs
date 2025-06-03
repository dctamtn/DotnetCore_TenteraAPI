using TenteraAPI.Domain.Entities;

namespace TenteraAPI.Domain.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task AddAsync(Account customer);
        Task<Account> GetICNumberAsync(string iCNumber);
        Task<bool> ICNumberExistsAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
        Task UpdateAsync(Account customer);
    }
}
