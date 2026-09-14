using System;
using System.Collections.Generic;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Domain.RepositoryInterfaces;

public interface IResidentRequestRepository
{
    long Create(ResidentRequest request);
    ResidentRequest? GetById(long id);
    IEnumerable<ResidentRequest> GetByUser(long userId, string? statusFilter = null);
    IEnumerable<ResidentRequest> GetByBuilding(long buildingId, string? statusFilter = null);
    bool HasPendingRequest(long userId, long buildingId, int apartmentNumber);
    void Delete(long id);
    void UpdateStatus(long id, RequestStatus status, long? handledBy = null, string? rejectionReason = null);
    void Update(ResidentRequest request);
}
