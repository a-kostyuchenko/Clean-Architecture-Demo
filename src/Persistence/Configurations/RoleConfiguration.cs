using Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Constants;

namespace Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(TableNames.Roles);
        
        builder.HasKey(x => x.Id);

        builder
            .HasMany(x => x.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>();

        builder
            .HasMany(x => x.Users)
            .WithMany(x => x.Roles);

        builder.HasData(Role.GetValues());
    }
}