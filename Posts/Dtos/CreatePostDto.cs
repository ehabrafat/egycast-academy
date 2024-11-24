using System.ComponentModel.DataAnnotations;

namespace EgycastApi.Posts.Dtos;

public class CreatePostDto
{

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public bool Pinned { get; set; } = false;
    
}