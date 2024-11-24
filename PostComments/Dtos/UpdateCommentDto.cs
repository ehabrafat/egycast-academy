using System.ComponentModel.DataAnnotations;

namespace EgycastApi.PostComments.Dtos;

public class UpdateCommentDto
{
    [Required]
    public string Content { get; set; }
}