using Finance.Domain.Models;
using Finance.Persistance.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class ResourceOwnerConfiguration : AuditedEntityConfiguration<ResourceOwner, Guid>, IEntityTypeConfiguration<ResourceOwner>
{
    public override void Configure(EntityTypeBuilder<ResourceOwner> builder)
    {
        base.Configure(builder);
        builder.HasOne(ro => ro.User)
            .WithMany()
            .HasForeignKey(ro => ro.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ro => ro.Resource)
            .WithMany()
            .HasForeignKey(ro => ro.ResourceId)
            .OnDelete(DeleteBehavior.Cascade); // Changed from Restrict to Cascade

        builder
            .HasIndex(ro => new { ro.UserId, ro.ResourceId })
            .IsUnique();
    }
}