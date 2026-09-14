using System;
using System.Collections.Generic;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Domain.RepositoryInterfaces;

public interface IBuildingRepository
{
    void Save(Building building);
    Building? GetById(long id);
    List<Building> GetAll(bool sortByFloors = false, bool onlyApproved = false);
    List<Building> GetByManagerJmbg(string managerJmbg);
    List<Building> SearchByAddress(string queryText);
    List<Building> SearchByNeighbourhood(string queryText);
    List<Building> SearchByFloors(int floors);
    List<Building> SearchByApartmentsAdvanced(int rooms, int residents, string op);
    bool ExistsApartmentNumberInBuilding(long apartmentId, long buildingId);
    void Update(Building building);
}
