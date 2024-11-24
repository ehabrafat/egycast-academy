using System.ComponentModel.DataAnnotations;
using EgycastApi.Community;
using EgycastApi.Posts;

namespace EgycastApi.PostLikes;

public class PostLike : BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string PostId { get; set; }
    
    public string CreatorId { get; set; }
    
    public Post Post { get; set; }
    
    public AppUser Creator { get; set; }
}