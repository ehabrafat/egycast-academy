using System.ComponentModel.DataAnnotations;
using EgycastApi.Community;
using EgycastApi.Posts;

namespace EgycastApi.PostComments;

public class PostComment : BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Content { get; set; }

    public bool Edited { get; set; }
    
    public AppUser Creator { get; set; }

    public Post Post { get; set; }

    public PostComment? ReplyToComment { get; set; }
    
    public string? ReplyToCommentId { get; set; }
}