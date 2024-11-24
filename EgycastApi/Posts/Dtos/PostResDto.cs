using EgycastApi.Auth.Dtos;
using EgycastApi.Storage.Dtos;

namespace EgycastApi.Posts.Dtos;

public class PostResDto
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string CommunityId { get; set; }

    public string Content { get; set; }

    public bool Pinned { get; set; }

    public int Likes { get; set; }
    
    public int Comments { get; set; }
    
    public bool LikedByMe { get; set; }
    public CreatorResDto Creator { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}