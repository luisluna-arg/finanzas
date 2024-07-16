using FinanceApi.Domain.Models;

namespace FinanceApi.Helpers.ExcelHelper;

public interface ICreditCardExcelHelper<TResult>
{
    IEnumerable<TResult> Read(IEnumerable<IFormFile> files, CreditCard creditCard, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);

    IEnumerable<TResult> Read(IFormFile file, CreditCard creditCard, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);
}
