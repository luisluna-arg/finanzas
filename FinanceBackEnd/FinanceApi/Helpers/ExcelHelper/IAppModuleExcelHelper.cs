using FinanceApi.Domain.Models;

namespace FinanceApi.Helpers.ExcelHelper;

public interface IAppModuleExcelHelper<TResult>
{
    IEnumerable<TResult> Read(IEnumerable<IFormFile> files, AppModule appModule, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);
    IEnumerable<TResult> Read(IFormFile file, AppModule appModule, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);
}
