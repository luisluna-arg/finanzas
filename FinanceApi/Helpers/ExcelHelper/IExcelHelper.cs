using FinanceApi.Domain.Models;

public interface IExcelHelper<TResult>
{
    public IEnumerable<TResult> ReadAsync(IEnumerable<IFormFile> files, AppModule appModule, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);

    public IEnumerable<TResult> ReadAsync(IFormFile file, AppModule appModule, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);
}
