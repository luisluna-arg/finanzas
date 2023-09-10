public interface IExcelHelper<TResult>
{
    public IEnumerable<TResult> ReadAsync(IEnumerable<IFormFile> files, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);

    public IEnumerable<TResult> ReadAsync(IFormFile file, DateTimeKind dateTimeKind = DateTimeKind.Unspecified);
}
