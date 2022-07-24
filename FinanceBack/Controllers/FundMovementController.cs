using FinanceBack.Models;
using FinanceBack.Services;

namespace FinanceBack.Controllers
{
    public class FundMovementController : BsonController<FundMovementService, FundMovement>
    {
        public FundMovementController(FundMovementService fundsService) : base(fundsService)
        {

        }

    }
}

