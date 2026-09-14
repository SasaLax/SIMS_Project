using CommunityHub.Application.Domain;

namespace CommunityHub.Application.DTO.DTOMappers;

public static class ApartmentDTOMapper
{
    public static ApartmentDTO ToDto(Apartment apartment)
    {
        return new ApartmentDTO
        {
            Id = apartment.Id,
            ApartmentNumber = apartment.ApartmentNumber,
            Description = apartment.Description,
            NumberOfRooms = apartment.NumberOfRooms,
            MaxNumberOfResidents = apartment.MaxNumberOfResidents,
            BuildingId = apartment.BuildingId
        };
    }
}
