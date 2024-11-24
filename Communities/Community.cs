using System.ComponentModel.DataAnnotations;
using EgycastApi.Communities.Utils;
using EgycastApi.Community;

namespace EgycastApi.Communities;

public class Community : BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string Title { get; set; }

    public string Description { get; set; }
    
    public string ImgUrl { get; set; }
    
    [EnumDataType(typeof(CommunityType))]
    public CommunityType Type { get; set; }

    public int CostPerMonth { get; set; }
}