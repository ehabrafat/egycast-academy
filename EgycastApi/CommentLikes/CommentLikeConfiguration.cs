using EgycastApi.PostLikes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EgycastApi.CommentLikes;

public class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLike>
{
    public void Configure(EntityTypeBuilder<CommentLike> builder)
    {
        builder.HasIndex(p => new { p.CommentId, p.CreatorId })
            .IsUnique();
        
        builder.HasOne(x => x.Creator)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.Comment)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}