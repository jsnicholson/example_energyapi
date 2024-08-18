using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Data.Repositories {
    public class AccountRepository : IAccountRepository {
        private readonly ILogger<AccountRepository> _logger;
        private readonly ApplicationDbContext _context;

        public AccountRepository(ILogger<AccountRepository> logger, ApplicationDbContext context) {
            _logger = logger;
            _context = context;
        }

        public Account? Read(int accountId) {
            return _context.Accounts.Where(a => a.AccountId == accountId).FirstOrDefault();
        }

        public IEnumerable<Account?> Read(IEnumerable<int> accountIds) {
            return _context.Accounts.Where(a => accountIds.Contains(a.AccountId)); 
        }
    }
}