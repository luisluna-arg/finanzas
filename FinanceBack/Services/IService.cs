
using FinanceBack.Models;

namespace FinanceBack.Services
{
    public interface IMongoDBAsyncService<Domain> where Domain : BsonDomain
    {
        public Task<List<Domain>> GetAsync();

        public Task<Domain?> GetAsync(string id);

        public Task CreateAsync(Domain newDomain);

        public Task UpdateAsync(string id, Domain updateDomain);

        public Task RemoveAsync(string id);
    }
}
