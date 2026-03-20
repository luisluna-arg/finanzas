using Microsoft.AspNetCore.Http;

public interface IExcelHelper<TResult>
{
    IEnumerable<TResult> Read(IEnumerable<IFormFile> files, short? timezoneOffset = null);

    IEnumerable<TResult> Read(IFormFile file, short? timezoneOffset = null);
}
