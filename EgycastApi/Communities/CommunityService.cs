using EgycastApi.Communities.Dtos;
using EgycastApi.Communities;
using EgycastApi.Community;
using EgycastApi.PostComments.Dtos;
using EgycastApi.Posts;
using EgycastApi.Posts.Dtos;
using EgycastApi.Storage.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EgycastApi.Communities;

public class CommunityService
{
    private readonly ApiDbContext _dbContext;
    private readonly PostService _postService;

    public CommunityService(ApiDbContext dbContext, PostService postService)
    {
        _dbContext = dbContext;
        _postService = postService;
    }

    public async Task<PaginatedResponse<CommunityResDto>> GetCommunities(int pageNum, int pageSize)
    {
        var totalItems = await _dbContext.Communities.CountAsync();
        var totalPages = (int)Math.Ceiling(1.00 * totalItems / pageSize);
        var items = await _dbContext.Communities
            .AsNoTracking()
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var communities = items.Select((c => c.ToResDto())).ToList();
        return new PaginatedResponse<CommunityResDto>
        {
            Items = communities,
            TotalPages = totalPages,
            TotalItems = totalItems,
            CurrentPage = pageNum,
            PageSize = pageSize
        };
    }


    public async Task<Community?> GetCommunity(string id)
    {
        return await _dbContext.Communities.FirstOrDefaultAsync(community => community.Id == id);
    }
    
    public async Task<Community> CreateCommunity(CreateCommunityDto communityDto)
    {
        var community = communityDto.ToCommunity();
        _dbContext.Communities.Add(community);
        await _dbContext.SaveChangesAsync();
        return community;
    }
    
    
    // POSTS
  public async Task CreatePostV2(string communityId, CreatePostDtoV2 postDto, CancellationToken cancellationToken)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.CreatePostV2(community, postDto, cancellationToken);
    }

    public async Task UpdatePost(string communityId, UpdatePostDto postDto)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.UpdatePost(community, postDto);
    }


    public async Task<List<FileResDto>> GetPostFiles(string communityId, string postId)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        var files = await _postService.GetPostFiles(postId);
        return files;
    }
    public async Task<PostResDto> GetPost(string communityId, string postId)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        var postRes = await _postService.GetPost(community, postId);
        return postRes;
    }
    public async Task<PaginatedResponse<PostResDto>> GetPosts(string communityId, int pageNum, int pageSize)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        return await _postService.GetPosts(community, pageNum, pageSize);
    }

    public async Task DeletePost(string communityId, string postId )
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.DeletePost(community, postId);
    }
    
    // LIKES
    public async Task LikePost(string communityId, string postId)
    {
        var communityExists = await _dbContext.Communities.AnyAsync(c => c.Id == communityId);
        if (!communityExists) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.LikePost(postId);
    }

    public async Task DeletePostLike(string communityId, string postId)
    {
        var communityExists = await _dbContext.Communities.AnyAsync(c => c.Id == communityId);
        if (!communityExists) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.DeletePostLike(postId);
    }
    // END LIKES
    
    // COMMENTS
    public async Task<PaginatedResponse<PostCommentResDto>> GetComments(string communityId, string postId, string? replyToCommentId, int pageNum, int pageSize)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        return await _postService.GetComments(postId, replyToCommentId, pageNum, pageSize);
    }
    
    public async Task AddComment(string communityId, string postId, CreateCommentDto commentDto, string? replyToCommentId)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.AddComment( postId, commentDto, replyToCommentId);
    }
    public async Task DeleteComment(string communityId, string postId, string commentId)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.DeleteComment( postId, commentId);
    }
    
    public async Task UpdateComment(string communityId, string postId, string commentId, UpdateCommentDto commentDto)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.UpdateComment(postId, commentId, commentDto);
    }
    // COMMENTS LIKES
    public async Task LikeComment(string communityId, string postId, string commentId)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.LikeComment(postId, commentId);

    }
    public async Task DislikeComment(string communityId, string postId, string commentId)
    {
        var community = await _dbContext.Communities.FirstOrDefaultAsync(c => c.Id == communityId);
        if (community is null) throw new EgycastException("Community not found", StatusCodes.Status404NotFound);
        await _postService.DislikeComment(postId, commentId);

    }
    // END COMMENTS LIKES
    
    // END COMMENTS
    // END POSTS

}