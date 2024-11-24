using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EgycastApi.PostLikes;

public class PostLikeConfiguration: IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.HasIndex(p => new { p.PostId, p.CreatorId })
            .IsUnique();
        
            builder.HasOne(x => x.Creator)
                    .WithMany()
                    .OnDelete(DeleteBehavior.NoAction);
            
            builder.HasOne(x => x.Post)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
    }
}