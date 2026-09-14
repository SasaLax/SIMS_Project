using CommunityHub.Application.Domain;

namespace CommunityHub.Application.DTO.DTOMappers;

public class ApartmentMapper : IMapper<Apartment, ApartmentDTO>
{
    public ApartmentDTO Map(Apartment source) => ApartmentDTOMapper.ToDto(source);
}
