using System.ComponentModel.DataAnnotations;

namespace EgycastApi.PostComments.Dtos;

public class CreateCommentDto
{
    
    [Required]
    public string Content { get; set; }
    
}