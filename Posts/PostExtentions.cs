using EgycastApi.Auth.Dtos;
using EgycastApi.Posts.Dtos;

namespace EgycastApi.Posts;

public static class PostExtentions
{
    public static Post ToPost(this CreatePostDto postDto)
    {
        return new Post
        {
            Title = postDto.Title,
            Content = postDto.Content,
            Pinned = postDto.Pinned,
        };
    }

    public static PostResDto ToResDto(this Post post)
    {
        return new PostResDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Pinned = post.Pinned,
            Creator = new CreatorResDto{Id = post.Creator.Id, Username = post.Creator.UserName, ImgUrl = post.Creator.ImgUrl},
            CommunityId = post.Community.Id,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}