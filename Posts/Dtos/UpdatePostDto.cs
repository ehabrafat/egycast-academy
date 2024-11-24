using System.ComponentModel.DataAnnotations;

namespace EgycastApi.Posts.Dtos;

public class UpdatePostDto
{
    [Required] 
    public string Id { get; set; }
    
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public bool Pinned { get; set; } = false;
}