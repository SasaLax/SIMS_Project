using CommunityHub.Application.Domain;

namespace CommunityHub.Application.DTO.DTOMappers;

public static class BuildingDTOMapper
{
    public static BuildingDTO ToDto(Building building)
    {
        return new BuildingDTO
        {
            Id = building.Id,
            BuildingCode = building.BuildingCode,
            Street = building.Address.Street,
            Number = building.Address.Number,
            Neighbourhood = building.Neighbourhood,
            City = building.Location.City,
            Country = building.Location.Country,
            NumberOfFloors = building.NumberOfFloors,
            ManagerJmbg = building.ManagerJmbg,
            Status = building.Status.ToString()
        };
    }
}
