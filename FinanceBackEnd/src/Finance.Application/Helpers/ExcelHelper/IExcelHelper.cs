using Microsoft.AspNetCore.Http;

public interface IExcelHelper<TResult>
{
    IEnumerable<TResult> Read(IEnumerable<IFormFile> files, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);

    IEnumerable<TResult> Read(IFormFile file, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);
}
