using FinanceApi.Domain.Models;

public interface IFundsExcelHelper<TResult>
{
    public IEnumerable<TResult> ReadAsync(IEnumerable<IFormFile> files, AppModule appModule, Bank? bank, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);

    public IEnumerable<TResult> ReadAsync(IFormFile file, AppModule appModule, Bank? bank, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);
}
