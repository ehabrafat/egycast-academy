using System.Text.Json;
using EgycastApi.Communities.Dtos;
using EgycastApi.Community;
using EgycastApi.PostComments.Dtos;
using EgycastApi.Posts;
using EgycastApi.Posts.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EgycastApi.Communities;

[ApiController]
[Route("communities")]
[Authorize(Policy = "authenticated")]
public class CommunityController : ControllerBase
{

    private readonly CommunityService _communityService;
    private readonly PostService _postService;

    public CommunityController(CommunityService communityService, PostService postService)
    {
        _communityService = communityService;
        _postService = postService;
    }
    
    [HttpPost]
    public Task<Community> CreateCommunity([FromBody] CreateCommunityDto communityDto)
    {
        return _communityService.CreateCommunity(communityDto);
    }
    
    [HttpGet]
    public Task<PaginatedResponse<CommunityResDto>> GetCommunities([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10)
    {
        return _communityService.GetCommunities(pageNum, pageSize);
    }
    [HttpGet("{id}")]
    public async Task<IResult> GetCommunity(string id)
    {
        var community = await _communityService.GetCommunity(id);
        if (community is null) return Results.NotFound();
        return Results.Ok(community);
    }

    // POSTS 
    [HttpPost("{id}/posts")]
    public async Task<IActionResult> CreatePost(string id, [FromForm] CreatePostDtoV2 body,CancellationToken cancellationToken)
    {
        await _communityService.CreatePostV2(id, body, cancellationToken);
        return Ok("Post Added");
    }
    
    [HttpPut("{id}/posts")]
    public async Task<IResult> UpdatePost(string id, [FromBody] UpdatePostDto postDto)
    {
        await _communityService.UpdatePost(id, postDto);
        return Results.Ok();
    }
    
    [HttpGet("{id}/posts")]
    public async Task<IResult> GetPosts(string id, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10)
    {
        var posts = await _communityService.GetPosts(id, pageNum, pageSize);
        return Results.Ok(posts);
    }

    [HttpGet("{id}/posts/{postId}/files")]
    public async Task<IResult> GetPostFiles(string id, string postId)
    {
        var post = await _communityService.GetPostFiles(id, postId);
        return Results.Ok(post);
    }
    [HttpGet("{id}/posts/{postId}")]
    public async Task<IResult> GetPost(string id, string postId)
    {
        var post = await _communityService.GetPost(id, postId);
        return Results.Ok(post);
    }
    [HttpDelete("{id}/posts/{postId}")]
    public async Task<IResult> DeletePost(string id, string postId, CancellationToken cancellationToken)
    {
        await _communityService.DeletePost(id, postId);
        return Results.Ok();
    }
    
    // LIKES
    [HttpPost("{id}/posts/{postId}/likes")]
    public async Task<IResult> LikePost(string id, string postId)
    {
        await _communityService.LikePost(id, postId);
        return Results.Ok("Like added successfully");
    }
    
    [HttpDelete("{id}/posts/{postId}/likes")]
    public async Task<IResult> DeletePostLike(string id, string postId)
    {
        await _communityService.DeletePostLike(id, postId);
        return Results.Ok("Like deleted successfully");
    }
    // END LIKES
    
    // COMMENTS
    [HttpGet("{id}/posts/{postId}/comments")]
    public async Task<PaginatedResponse<PostCommentResDto>> GetComments(string id, string postId, 
        [FromQuery] string? replyToCommentId, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10)
    {
       var resComments = await _communityService.GetComments(id, postId, replyToCommentId, pageNum, pageSize);
       JsonSerializer.Serialize(resComments);
       return resComments;
    }
    
    
    [HttpPost("{id}/posts/{postId}/comments")]
    public async Task<IResult> AddComment(string id, string postId, [FromBody] CreateCommentDto commentDto, [FromQuery] string? replyToCommentId)
    {
        await _communityService.AddComment(id, postId, commentDto, replyToCommentId);
        return Results.Ok("Commend Added Successfully");
    }
    [HttpDelete("{id}/posts/{postId}/comments/{commentId}")]
    public async Task<IResult> DeleteComment(string id, string postId, string commentId)
    {
        await _communityService.DeleteComment(id, postId, commentId);
        return Results.Ok("Commend Deleted Successfully");
    }
    
    [HttpPut("{id}/posts/{postId}/comments/{commentId}")]
    public async Task<IResult> UpdateComment(string id, string postId, string commentId, UpdateCommentDto commentDto)
    {
        await _communityService.UpdateComment(id, postId, commentId, commentDto);
        return Results.Ok("Commend Updated Successfully");
    }
    // COMMENTS LIKES
    [HttpPost("{id}/posts/{postId}/comments/{commentId}/likes")]
    public async Task<IResult> LikeComment(string id, string postId, string commentId)
    {
        await _communityService.LikeComment(id, postId, commentId);
        return Results.Ok();
    }
    [HttpDelete("{id}/posts/{postId}/comments/{commentId}/likes")]
    public async Task<IResult> DislikeComment(string id, string postId, string commentId)
    {
        await _communityService.DislikeComment(id, postId, commentId);
        return Results.Ok();
    }
    // END COMMENTS LIKES
    // END COMMENTS
    // END POSTS
}