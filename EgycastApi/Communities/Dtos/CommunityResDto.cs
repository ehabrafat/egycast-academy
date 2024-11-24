namespace EgycastApi.Communities.Dtos;

public class CommunityResDto
{
    public string Id { get; set; }
    
    public string Title { get; set; }

    public string Description { get; set; }
    
    public string ImgUrl { get; set; }
    
    public string Type { get; set; }

    public int CostPerMonth { get; set; }
}