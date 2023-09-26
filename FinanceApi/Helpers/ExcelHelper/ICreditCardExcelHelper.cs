using FinanceApi.Domain.Models;

namespace FinanceApi.Helpers.ExcelHelper;

public interface ICreditCardExcelHelper<TResult>
{
    IEnumerable<TResult> Read(IEnumerable<IFormFile> files, CreditCardIssuer creditCardIssuer, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);

    IEnumerable<TResult> Read(IFormFile file, CreditCardIssuer creditCardIssuer, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);
}
