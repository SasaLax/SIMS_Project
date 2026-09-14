using System;
using System.Collections.Generic;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;

namespace CommunityHub.Application.Service;

public interface IBuildingAccessRequestService
{
    bool IsApartmentAlreadyOccupied(long buildingId, int apartmentNumber);
    long CreateRequest(long residentId, long buildingId, int apartmentNumber);
    IEnumerable<ResidentRequest> GetResidentRequests(long residentId, RequestStatus? status = null);
    void CancelRequest(long requestId, long residentId);
}
