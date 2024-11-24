using EgycastApi.Auth;
using EgycastApi.Auth.Dtos;
using EgycastApi.CommentLikes;
using EgycastApi.Communities.Dtos;
using EgycastApi.Community;
using EgycastApi.PostComments.Dtos;
using EgycastApi.PostLikes;
using EgycastApi.Posts;
using EgycastApi.Users;
using Microsoft.EntityFrameworkCore;

namespace EgycastApi.PostComments;

public class PostCommentService
{
    private readonly ApiDbContext _dbContext;
    private readonly AuthService _authService;

    public PostCommentService(ApiDbContext dbContext, AuthService authService)
    {
        _dbContext = dbContext;
        _authService = authService;
    }

    public async Task<PaginatedResponse<PostCommentResDto>> GetComments(string postId, string? replyToCommentId, int pageNum, int pageSize)
    {

        var comments = await _dbContext.PostComments
                .Include(x => x.Creator)
                .Include(x => x.Post)
                .Where(x => x.Post.Id == postId && x.ReplyToCommentId == replyToCommentId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        var totalItems = await _dbContext.PostComments.CountAsync(x => x.Post.Id == postId && x.ReplyToCommentId == replyToCommentId);
        var totalPages = (int)Math.Ceiling(1.00 * totalItems / pageSize);
        var resComments = comments.Select(comment => new PostCommentResDto
        {
            Id = comment.Id,
            Content = comment.Content,
            Edited = comment.Edited,
            Creator = new CreatorResDto
            {
                Id = comment.Creator.Id,
                Username = comment.Creator.UserName,
                ImgUrl = comment.Creator.ImgUrl
            },
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            
        }).ToList();
        var userId = _authService.GetUserClaim("id");
        foreach (var comment in resComments)
        {
            comment.Likes = await _dbContext.CommentLikes.CountAsync(x => x.CommentId == comment.Id);
            comment.LikedByMe =
                (await _dbContext.CommentLikes.FirstOrDefaultAsync(x =>
                    x.CommentId == comment.Id && x.CreatorId == userId)) is not null;
        }
        
        var response = new PaginatedResponse<PostCommentResDto>
        {
            Items = resComments,
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = pageNum,
            PageSize = pageSize
        };
        
        return response;
    }
    
    
    public async Task Create(Post post, CreateCommentDto commentDto, string? replyToCommentId)
    {
        var userId = _authService.GetUserClaim("id");
        var creator = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (creator is null) throw new EgycastException("User not found", StatusCodes.Status404NotFound);
        var replyToComment = replyToCommentId is not null ? (await _dbContext.PostComments.FirstOrDefaultAsync(x => x.Id == replyToCommentId)) : null;
        var postComment = new PostComment
        {
            Content = commentDto.Content,
            Creator = creator,
            Post = post,
            ReplyToComment = replyToComment
        };
        _dbContext.PostComments.Add(postComment);
        await _dbContext.SaveChangesAsync();
    }
    public async Task Update(string commentId, UpdateCommentDto commentDto)
    {
        var userId = _authService.GetUserClaim("id");
        var creator = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (creator is null) throw new EgycastException("User not found", StatusCodes.Status404NotFound);
        var comment = await _dbContext.PostComments
            .Include(x => x.Creator)
            .FirstOrDefaultAsync(x =>
            x.Id == commentId);
        if (comment is null) throw new EgycastException("Comment not found", StatusCodes.Status404NotFound);
        if (userId != comment.Creator.Id) throw new EgycastException("Forbidden", StatusCodes.Status403Forbidden);
        comment.Content = commentDto.Content;
        comment.Edited = true;
        _dbContext.PostComments.Update(comment);
        await _dbContext.SaveChangesAsync();
    }
    public async Task Delete(string commentId)
    {
        await _dbContext.PostComments.Where(x => x.ReplyToCommentId == commentId)
            .ExecuteDeleteAsync();
        await _dbContext.PostComments.Where(x => x.Id == commentId)
            .ExecuteDeleteAsync();
    }

    public async Task Like(string commentId)
    {
        var userId = _authService.GetUserClaim("id");
        var entity = await _dbContext.CommentLikes.FirstOrDefaultAsync(x => x.CommentId == commentId && x.CreatorId == userId);
        if (entity is not null)
        {
            throw new EgycastException("Comment like already added", StatusCodes.Status409Conflict);
        }

        var creator = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (creator is null) throw new EgycastException("User not found", StatusCodes.Status404NotFound);
        var comment = await _dbContext.PostComments.FirstOrDefaultAsync(x => x.Id == commentId);
        if (comment is null) throw new EgycastException("Comment not found", StatusCodes.Status404NotFound);
        var commentLike = new CommentLike
        {
            Creator = creator,
            Comment = comment
        };
        _dbContext.CommentLikes.Add(commentLike);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task Dislike(string commentId)
    {
        var userId = _authService.GetUserClaim("id");
        var entity = await _dbContext.CommentLikes.FirstOrDefaultAsync(x => x.CommentId == commentId && x.CreatorId == userId);
        if (entity is null)
        {
            throw new EgycastException("Comment like not found", StatusCodes.Status404NotFound);
        }
        var creator = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (creator is null) throw new EgycastException("User not found", StatusCodes.Status404NotFound);
        _dbContext.CommentLikes.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> GetCommentsCount(string postId)
    {
        return await _dbContext.PostComments
            .Include(x => x.Post)
            .CountAsync(x => x.Post.Id == postId);
    }
}