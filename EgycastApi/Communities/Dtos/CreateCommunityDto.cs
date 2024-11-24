using System.ComponentModel.DataAnnotations;
using EgycastApi.Communities.Utils;
using EgycastApi.Community;

namespace EgycastApi.Communities.Dtos;

public class CreateCommunityDto
{
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    public string ImgUrl { get; set; }
    
    [Required, ValidEnum(typeof(CommunityType))]
    public string Type { get; set; }

    public int CostPerMonth { get; set; }
}