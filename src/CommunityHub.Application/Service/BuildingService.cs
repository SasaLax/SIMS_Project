using System.Collections.Generic;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;

namespace CommunityHub.Application.Service;

public interface IBuildingService
{
    List<Building> GetAll(bool sortByFloors = false, bool onlyApproved = false);
    Building? GetById(long id);
    List<Building> SearchByAddress(string queryText);
    List<Building> SearchByNeighbourhood(string queryText);
    List<Building> SearchByFloors(int floors);
    List<Building> SearchByApartmentsAdvanced(int rooms, int residents, string op);
    bool ExistsApartmentNumberInBuilding(int apartmentNumber, long buildingId);
}

public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IApartmentRepository _apartmentRepository;

    public BuildingService(IBuildingRepository buildingRepository, IApartmentRepository apartmentRepository)
    {
        _buildingRepository = buildingRepository;
        _apartmentRepository = apartmentRepository;
    }

    public List<Building> GetAll(bool sortByFloors = false, bool onlyApproved = false)
        => _buildingRepository.GetAll(sortByFloors, onlyApproved);

    public Building? GetById(long id) => _buildingRepository.GetById(id);

    public List<Building> SearchByAddress(string queryText)
        => _buildingRepository.SearchByAddress(queryText);

    public List<Building> SearchByNeighbourhood(string queryText)
        => _buildingRepository.SearchByNeighbourhood(queryText);

    public List<Building> SearchByFloors(int floors)
        => _buildingRepository.SearchByFloors(floors);

    public List<Building> SearchByApartmentsAdvanced(int rooms, int residents, string op)
        => _buildingRepository.SearchByApartmentsAdvanced(rooms, residents, op);

    public bool ExistsApartmentNumberInBuilding(int apartmentNumber, long buildingId)
        => _apartmentRepository.ExistsApartmentNumberInBuilding(apartmentNumber, buildingId);
}
