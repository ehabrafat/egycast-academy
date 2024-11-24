using EgycastApi.CommentLikes;
using EgycastApi.PostComments;
using EgycastApi.PostLikes;
using EgycastApi.Posts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace EgycastApi;

public class ApiDbContext : IdentityDbContext<AppUser>
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(typeof(ApiDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified).ToList();
        
        foreach (var e in entities)
        {
            if (e.State == EntityState.Added)
            {
                e.Property("CreatedAt").CurrentValue = DateTime.Now;
                e.Property("UpdatedAt").CurrentValue = DateTime.Now;
            } else if (e.State == EntityState.Modified)
            {
                e.Property("CreatedAt").IsModified = false;
                e.Property("UpdatedAt").CurrentValue = DateTime.Now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
        
    }
    
    public DbSet<Communities.Community> Communities { get; set; }
    
    public DbSet<Post> Posts { get; set; }

    public DbSet<PostLike> PostLikes { get; set; }
    
    public DbSet<CommentLike> CommentLikes { get; set; }

    public DbSet<PostComment> PostComments { get; set; }
}