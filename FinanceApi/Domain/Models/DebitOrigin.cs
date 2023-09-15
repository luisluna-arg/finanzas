using System.ComponentModel.DataAnnotations.Schema;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public sealed class DebitOrigin : Entity<Guid>, IEquatable<DebitOrigin>
{
    [ForeignKey("AppModuleId")]
    public Guid AppModuleId { get; set; }

    public AppModule AppModule { get; set; }

    required public string Name { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as DebitOrigin);
    }

    public bool Equals(DebitOrigin? origin)
    {
        if (origin is null) return false;

        return AppModuleId == origin.AppModuleId &&
            Name == origin.Name;
    }

    public override int GetHashCode() => (
        AppModuleId,
        Name).GetHashCode();
}
