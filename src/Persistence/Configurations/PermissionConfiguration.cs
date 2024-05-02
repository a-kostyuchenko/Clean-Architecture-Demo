using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Constants;
using Permission = Domain.Roles.Permission;

namespace Persistence.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(TableNames.Permissions);
        
        builder.HasKey(x => x.Id);

        var permissions = Domain.AssemblyReference.Assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsClass: true } &&
                           type.IsSubclassOf(typeof(SharedKernel.Permission)))
            .SelectMany(type => type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fieldInfo => typeof(SharedKernel.Permission).IsAssignableFrom(fieldInfo.FieldType))
                    .Select(fieldInfo => (SharedKernel.Permission)fieldInfo.GetValue(default)!)
        .Select(permission => Permission.Create(
            permission.Key,
            permission.Name,
            permission.Description))
        .ToList());
        
        builder.HasData(permissions);
    }
}