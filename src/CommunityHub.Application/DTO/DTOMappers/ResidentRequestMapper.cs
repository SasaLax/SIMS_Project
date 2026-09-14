using CommunityHub.Application.Domain;

namespace CommunityHub.Application.DTO.DTOMappers;

public class ResidentRequestMapper : IMapper<ResidentRequest, ResidentRequestDTO>
{
    public ResidentRequestDTO Map(ResidentRequest source) => ResidentRequestDTOMapper.ToDto(source);
}
