using EgycastApi.Auth.Dtos;
using EgycastApi.Community;

namespace EgycastApi.PostComments.Dtos;

public class PostCommentResDto : BaseEntity
{

    public string Id { get; set; }
    public string Content { get; set; }

    public int Likes { get; set; }
    
    public bool LikedByMe { get; set; }

    public bool Edited { get; set; }
    
    public CreatorResDto Creator { get; set; }
}   