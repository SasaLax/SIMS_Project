using CommunityHub.Application.Domain;

namespace CommunityHub.Application.DTO.DTOMappers;

public class BuildingMapper : IMapper<Building, BuildingDTO>
{
    public BuildingDTO Map(Building source) => BuildingDTOMapper.ToDto(source);
}
