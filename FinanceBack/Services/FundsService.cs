using FinanceBack.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FinanceBack.Services
{
    public class FundMovementService : IMongoDBAsyncService<FundMovement>
    {
        private readonly IMongoCollection<FundMovement> _fundsMovementsCollection;

        public FundMovementService(IOptions<FinanceDatabaseSettings> financeDatabaseSettings)
        {
            var mongoClient = new MongoClient(financeDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(financeDatabaseSettings.Value.DatabaseName);

            _fundsMovementsCollection = mongoDatabase.GetCollection<FundMovement>(financeDatabaseSettings.Value.FundsCollectionName);
        }

        public async Task<List<FundMovement>> GetAsync() =>
            await _fundsMovementsCollection.Find(_ => true).ToListAsync();

        public async Task<FundMovement?> GetAsync(string id) =>
            await _fundsMovementsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(FundMovement newMovement) =>
            await _fundsMovementsCollection.InsertOneAsync(newMovement);

        public async Task UpdateAsync(string id, FundMovement updatedMovement) =>
            await _fundsMovementsCollection.ReplaceOneAsync(x => x.Id == id, updatedMovement);

        public async Task RemoveAsync(string id) =>
            await _fundsMovementsCollection.DeleteOneAsync(x => x.Id == id);
    }
}