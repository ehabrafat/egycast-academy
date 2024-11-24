using System.Text;
using System.Text.Json.Serialization;
using Amazon.S3;
using EgycastApi;
using EgycastApi.Auth;
using EgycastApi.CommentLikes;
using EgycastApi.Communities;
using EgycastApi.Community;
using EgycastApi.Config;
using EgycastApi.PostComments;
using EgycastApi.PostLikes;
using EgycastApi.Posts;
using EgycastApi.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000")
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader();

    });
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApiDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var jwtConfig = builder.Configuration.GetRequiredSection("Jwt").Get<JwtConfig>();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetRequiredSection("Jwt"));
builder.Services.AddAuthentication("jwt")
    .AddJwtBearer("jwt", options =>
    {
        options.Events = new()
        {
            OnMessageReceived = context =>
            {
                context.Token = context.HttpContext.Request.Cookies["accessToken"];
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig!.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authenticated", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes("jwt");
    });
    
});
builder.Services.Configure<S3Config>(builder.Configuration.GetSection("S3"));
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddSingleton<FileService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CommunityService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<PostLikeService>();
builder.Services.AddScoped<PostCommentService>();

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors("AllowSpecificOrigin");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.Run();