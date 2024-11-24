using EgycastApi.Auth;
using EgycastApi.Auth.Dtos;
using EgycastApi.Community;
using EgycastApi.Config;
using EgycastApi.PostComments;
using EgycastApi.PostComments.Dtos;
using EgycastApi.PostLikes;
using EgycastApi.Posts.Dtos;
using EgycastApi.Storage;
using EgycastApi.Storage.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace EgycastApi.Posts;

public class PostService
{
    private readonly ApiDbContext _dbContext;
    private readonly AuthService _authService;
    private readonly PostLikeService _PostLikeservice;
    private readonly PostCommentService _postCommentService;
    private readonly FileService _fileService;
    private readonly S3Config _s3Config;

    public PostService(ApiDbContext dbContext, AuthService authService, PostLikeService PostLikeservice,
        PostCommentService postCommentService, FileService fileService,
        IOptions<S3Config> s3Config)
    {
        _dbContext = dbContext;
        _authService = authService;
        _PostLikeservice = PostLikeservice;
        _postCommentService = postCommentService;
        _fileService = fileService;
        _s3Config = s3Config.Value;
    }

    
    public async Task CreatePostV2(Communities.Community community, CreatePostDtoV2 postDto, CancellationToken cancellationToken)
    {
        var user = await _authService.GetCurrentUser();
        var post = new Post
        {
            Title = postDto.Title,
            Content = postDto.Content,
            Pinned = postDto.Pinned,
            Creator = user,
            Community = community
        };
        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync();
        await _fileService.UploadFiles(postDto.files, _s3Config.Bucket, $"posts/{post.Id}", cancellationToken);
    }
    
    public async Task UpdatePost(Communities.Community community, UpdatePostDto postDto)
    {
        var user = await _authService.GetCurrentUser();
        if (user is null) throw new EgycastException("User not found", StatusCodes.Status404NotFound);
        var post = await _dbContext.Posts.FindAsync(postDto.Id);
        if(post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        post.Creator = user;
        post.Title = postDto.Title;
        post.Content = postDto.Content;
        post.Pinned = postDto.Pinned;
        _dbContext.Posts.Update(post);
        await _dbContext.SaveChangesAsync();
    }


    public async Task<List<FileResDto>> GetPostFiles(string postId)
    {
        var files = await _fileService.GetAllFiles(_s3Config.Bucket, $"posts/{postId}");
        return files;
    }
    public async Task<PostResDto> GetPost(Communities.Community community, string postId)
    {
        var post = await _dbContext.Posts
            .Include(p => p.Creator)
            .Include(p => p.Community)
            .Select(x => new {
                x.Id, 
                x.Title, 
                x.Content,
                x.Pinned,
                x.CreatedAt,
                x.UpdatedAt,
                CommunityId = x.Community.Id, 
                CreatorUsername = x.Creator.UserName, 
                CreatorId = x.Creator.Id, 
                CreatorImgUrl = x.Creator.ImgUrl})
            .FirstOrDefaultAsync(p => community.Id == p.CommunityId && p.Id == postId);
        if(post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        var postRes = new PostResDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Pinned = post.Pinned,
            Creator = new CreatorResDto
            {
                Username = post.CreatorUsername,
                ImgUrl = post.CreatorImgUrl,
                Id = post.CreatorId
            },
            CommunityId = community.Id,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
        var userId = _authService.GetUserClaim("id");
        postRes.Likes =  await _dbContext.PostLikes.CountAsync(postLike => postLike.PostId == post.Id);
        postRes.Comments = await _postCommentService.GetCommentsCount(postId);
        postRes.LikedByMe = (await _dbContext.PostLikes.FirstOrDefaultAsync(postLike => postLike.PostId == post.Id && postLike.CreatorId == userId)) is not null;
        return postRes;
    }
    
    public async Task<PaginatedResponse<PostResDto>> GetPosts(Communities.Community community, int pageNum, int pageSize)
    {
        var totalItems = await _dbContext.Posts.Include((x => x.Community)).CountAsync(x => x.Community.Id == community.Id);
        var totalItems = await _dbContext.Posts.CountAsync(x => x.Community.Id == community.Id);
        var totalPages = (int)Math.Ceiling(1.00 * totalItems / pageSize);
        var posts = await _dbContext.Posts
            .Where(post => post.Community.Id == community.Id)
            .Include(post => post.Creator)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var resPosts = posts.Select(post => post.ToResDto()).ToList();
        
        var userId = _authService.GetUserClaim("id");
        
        foreach (var post in resPosts)
        {
            post.Likes =  await _dbContext.PostLikes.CountAsync(postLike => postLike.PostId == post.Id);
            post.Comments = await _postCommentService.GetCommentsCount(post.Id);
            post.LikedByMe = (await _dbContext.PostLikes.FirstOrDefaultAsync(postLike => postLike.PostId == post.Id && postLike.CreatorId == userId)) is not null;
        }
        return new PaginatedResponse<PostResDto>
        {
            Items = resPosts,
            CurrentPage = pageNum,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task DeletePost(Communities.Community community, string postId)
    {
        var post = await _dbContext.Posts
            .Include(p => p.Creator)
            .Select(p => new {Id = p.Id, UserId = p.Creator.Id})
            .FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        var userId = _authService.GetUserClaim("id")!;
        if (userId != post.UserId) throw new EgycastException("Forbidden", StatusCodes.Status403Forbidden);
        await _dbContext.Posts.Where(p => p.Id == postId)
            .ExecuteDeleteAsync();
    }

    public async Task LikePost(string postId)
    {
        var userId = _authService.GetUserClaim("id")!;
        var postLikeExists = await _dbContext.PostLikes
            .AsNoTracking()
            .AnyAsync(x => x.PostId == postId && x.CreatorId == userId);
        if(postLikeExists) throw new EgycastException("Post like already added", StatusCodes.Status400BadRequest);
        await _PostLikeservice.Create(userId, postId);
    }

    public async Task DeletePostLike(string postId)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        var userId = _authService.GetUserClaim("id")!;
        await _PostLikeservice.Delete(postId, userId);
    }


    public async Task<PaginatedResponse<PostCommentResDto>> GetComments(string postId, string? replyToCommentId, int pageNum, int pageSize)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        return await _postCommentService.GetComments(postId, replyToCommentId, pageNum, pageSize);
    }
    public async Task AddComment(string postId, CreateCommentDto commentDto, string? replyToCommentId)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        await _postCommentService.Create(post, commentDto, replyToCommentId);
    }
    public async Task DeleteComment(string postId, string commentId)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        await _postCommentService.Delete(commentId);
    }
    public async Task UpdateComment(string postId, string commentId, UpdateCommentDto commentDto)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        await _postCommentService.Update(commentId, commentDto);
    }
    public async Task LikeComment(string postId, string commentId)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        await _postCommentService.Like(commentId);
    }
    public async Task DislikeComment(string postId, string commentId)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) throw new EgycastException("Post not found", StatusCodes.Status404NotFound);
        await _postCommentService.Dislike(commentId);
    }
}   