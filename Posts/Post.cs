using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EgycastApi.Posts;

public class Post
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Title { get; set; }

    public string Content { get; set; }

    public bool Pinned { get; set; }
    
    public Communities.Community Community { get; set; }
    
    public AppUser Creator { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
}