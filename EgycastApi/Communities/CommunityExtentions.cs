using EgycastApi.Communities.Dtos;
using EgycastApi.Communities.Utils;

namespace EgycastApi.Communities;

public static class CommunityExtentions
{
    public static Community ToCommunity(this CreateCommunityDto communityDto)
    {
        CommunityType.TryParse(communityDto.Type, ignoreCase: true, out CommunityType type);
        return new Community
        {
            Title = communityDto.Title,
            Description = communityDto.Description,
            Type = type,
            ImgUrl = communityDto.ImgUrl,
            CostPerMonth = communityDto.CostPerMonth
        };
    }

    public static CommunityResDto ToResDto(this Community community)
    {
        return new CommunityResDto
        {
            Id = community.Id,
            Title = community.Title,
            Description = community.Description,
            ImgUrl = community.ImgUrl,
            Type = community.Type.ToString().ToLower(),
            CostPerMonth = community.CostPerMonth,
        };
    }
}