using Finance.Domain.Models;
using Microsoft.AspNetCore.Http;

public interface IFundsExcelHelper<TResult>
{
    IEnumerable<TResult> Read(IEnumerable<IFormFile> files, AppModule appModule, Bank? bank, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);

    IEnumerable<TResult> Read(IFormFile file, AppModule appModule, Bank? bank, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);
}
