using System;
using System.Collections.Generic;
using System.Linq;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;

namespace CommunityHub.Application.Service;

public class BuildingAccessRequestService : IBuildingAccessRequestService
{
    private readonly IResidentRequestRepository _requestRepo;
    private readonly IApartmentRepository _apartmentRepo;
    private readonly IBuildingMembershipRepository? _membershipRepo;

    public BuildingAccessRequestService(IResidentRequestRepository requestRepo,
                                        IApartmentRepository apartmentRepo,
                                        IBuildingMembershipRepository? membershipRepo = null)
    {
        _requestRepo = requestRepo;
        _apartmentRepo = apartmentRepo;
        _membershipRepo = membershipRepo;
    }

    public bool IsApartmentAlreadyOccupied(long buildingId, int apartmentNumber)
    {
        if (_membershipRepo != null && _membershipRepo.Exists(buildingId, apartmentNumber))
        {
            return true;
        }

        return _requestRepo.GetByBuilding(buildingId, RequestStatus.Approved.ToString())
            .Any(r => r.ApartmentNumber == apartmentNumber);
    }

    public long CreateRequest(long residentId, long buildingId, int apartmentNumber)
    {
        if (_requestRepo.HasPendingRequest(residentId, buildingId, apartmentNumber))
        {
            throw new InvalidOperationException("Već imate aktivan zahtev za ovaj stan u ovoj zgradi.");
        }

        var req = ResidentRequest.CreatePending(residentId, buildingId, apartmentNumber);
        return _requestRepo.Create(req);
    }

    public IEnumerable<ResidentRequest> GetResidentRequests(long residentId, RequestStatus? status = null)
        => _requestRepo.GetByUser(residentId, status?.ToString());

    public void CancelRequest(long requestId, long residentId)
    {
        var r = _requestRepo.GetById(requestId);
        if (r == null || r.UserId != residentId) throw new InvalidOperationException("Not allowed");
        if (r.Status != RequestStatus.Pending) throw new InvalidOperationException("Only pending requests can be cancelled");
        _requestRepo.Delete(requestId);
    }
}
