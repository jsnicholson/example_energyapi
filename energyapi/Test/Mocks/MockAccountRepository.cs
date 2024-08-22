using Data.Entities;
using Data.Repositories.Interfaces;

namespace Test.Mocks {
    public class MockAccountRepository : IAccountRepository {
        private IDictionary<int, Account> _accounts = new Dictionary<int, Account>();

        public Account? Read(int accountId) {
            if(_accounts.ContainsKey(accountId)) {
                return _accounts[accountId];
            }
            return null;
        }

        public IEnumerable<Account?> Read(IEnumerable<int> accountIds) {
            return _accounts
                .Where(kv => accountIds.Contains(kv.Key))
                .Select(kv => kv.Value);
        }
    }
}