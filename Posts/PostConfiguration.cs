using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EgycastApi.Posts;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasOne(x => x.Creator)
            .WithMany()
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.Community)
            .WithMany()
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}