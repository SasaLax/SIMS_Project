using System;
using System.Collections.Generic;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Domain.RepositoryInterfaces;

public interface IApartmentRepository
{
    void Save(Apartment apartment);
    Apartment? GetById(long id);
    List<Apartment> GetByBuildingId(long buildingId);
    bool ExistsApartmentNumberInBuilding(int apartmentNumber, long buildingId);
}
