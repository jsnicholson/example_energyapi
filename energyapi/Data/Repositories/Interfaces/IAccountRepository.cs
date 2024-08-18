using Data.Entities;

namespace Data.Repositories.Interfaces {
    public interface IAccountRepository {
        Account? Read(int accountId);
        IEnumerable<Account?> Read(IEnumerable<int> accountIds);
    }
}