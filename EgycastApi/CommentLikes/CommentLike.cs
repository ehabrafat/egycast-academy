using System.ComponentModel.DataAnnotations;
using EgycastApi.Community;
using EgycastApi.PostComments;

namespace EgycastApi.CommentLikes;

public class CommentLike : BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string CommentId { get; set; }
    
    public string CreatorId { get; set; }
    
    public PostComment Comment { get; set; }
    
    public AppUser Creator { get; set; }
}