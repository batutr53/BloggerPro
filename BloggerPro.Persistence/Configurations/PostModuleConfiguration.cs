using BloggerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloggerPro.Persistence.Configurations;

public class PostModuleConfiguration : IEntityTypeConfiguration<PostModule>
{
    public void Configure(EntityTypeBuilder<PostModule> builder)
    {
        builder.HasKey(pm => pm.Id);

        builder.HasOne(pm => pm.Post)
               .WithMany(p => p.Modules)
               .HasForeignKey(pm => pm.PostId);

        builder.Property(pm => pm.Type).IsRequired();
        builder.Property(pm => pm.SortOrder).IsRequired();
    }
}
