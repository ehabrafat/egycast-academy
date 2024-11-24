using System.ComponentModel.DataAnnotations;

namespace EgycastApi.Posts.Dtos;

public class CreatePostDtoV2
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public List<IFormFile> files { get; set; } = new();

    public bool Pinned { get; set; } = false;
}