using Microsoft.EntityFrameworkCore;

namespace EgycastApi.PostLikes;

public class PostLikeService
{
    private readonly ApiDbContext _dbContext;

    public PostLikeService(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<PostLike?> FindById(string postId)
    {
        var postLike = await _dbContext.PostLikes
            .FirstOrDefaultAsync(postLike => postLike.PostId == postId);
        return postLike;
    }
    
    public async Task<PostLike?> FindByUserId(string postId, string creatorId)
    {
        var postLike = await _dbContext.PostLikes
            .Where(postLike => postLike.PostId == postId && postLike.CreatorId == creatorId)
            .FirstOrDefaultAsync();
        return postLike;
    }

    public async Task Delete(string postId, string creatorId)
    {
        var entity = await _dbContext.PostLikes
            .Where(postLike => postLike.PostId == postId && postLike.CreatorId == creatorId)
            .FirstOrDefaultAsync();
        if (entity is null) throw new EgycastException("Post like not found", StatusCodes.Status404NotFound);
        _dbContext.PostLikes.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Create(string creatorId, string postId)
    {
        var postLike = new PostLike { CreatorId = creatorId, PostId = postId };
        _dbContext.PostLikes.Add(postLike);
        await _dbContext.SaveChangesAsync();
    }
    
}