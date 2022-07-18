namespace FinanceBack.Models
{
    public class FinanceDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string FundsCollectionName { get; set; } = null!;
    }
}
