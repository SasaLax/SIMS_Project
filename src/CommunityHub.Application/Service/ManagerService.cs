using System;
using System.Collections.Generic;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;

namespace CommunityHub.Application.Service;

public interface IManagerService
{
    List<Building> GetMyBuildings(string managerJmbg);
    List<ResidentRequest> GetRequestsForBuilding(long buildingId, RequestStatus? status = null);
    void ApproveRequest(long requestId, long managerId);
    void RejectRequest(long requestId, long managerId, string reason);
    void ApproveBuilding(long buildingId);
    void RejectBuilding(long buildingId);
    void AddApartment(Apartment apartment);
    void AddApartmentWithValidation(Apartment apartment);
}

public class ManagerService : IManagerService
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IResidentRequestRepository _requestRepository;
    private readonly IApartmentRepository _apartmentRepository;

    public ManagerService(IBuildingRepository buildingRepository, IResidentRequestRepository requestRepository, IApartmentRepository apartmentRepository)
    {
        _buildingRepository = buildingRepository;
        _requestRepository = requestRepository;
        _apartmentRepository = apartmentRepository;
    }
    public List<Building> GetMyBuildings(string managerJmbg)
    {
        return _buildingRepository.GetByManagerJmbg(managerJmbg);
    }

    public List<ResidentRequest> GetRequestsForBuilding(long buildingId, RequestStatus? status = null)
    {
        return (List<ResidentRequest>)_requestRepository.GetByBuilding(buildingId, status?.ToString());
    }

    public void ApproveRequest(long requestId, long managerId)
    {
        var request = _requestRepository.GetById(requestId);
        if (request == null) throw new InvalidOperationException("Request not found");
        request.Approve(managerId);
        _requestRepository.UpdateStatus(requestId, request.Status, request.HandledByManagerId, request.RejectionReason);
    }

    public void RejectRequest(long requestId, long managerId, string reason)
    {
        var request = _requestRepository.GetById(requestId);
        if (request == null) throw new InvalidOperationException("Request not found");
        request.Reject(managerId, reason);
        _requestRepository.UpdateStatus(requestId, request.Status, request.HandledByManagerId, request.RejectionReason);
    }

    public void ApproveBuilding(long buildingId)
    {
        var building = _buildingRepository.GetById(buildingId);
        if (building == null)
            throw new InvalidOperationException("Zgrada nije pronađena.");

        if (building.Status != BuildingStatus.Pending)
            throw new InvalidOperationException("Možete odobriti samo zgrade koje su na čekanju.");

        building.Approve();
        _buildingRepository.Update(building);
    }

    public void RejectBuilding(long buildingId)
    {
        var building = _buildingRepository.GetById(buildingId);
        if (building == null) throw new InvalidOperationException("Building not found");
        building.Reject();
        _buildingRepository.Update(building);
    }

    public void AddApartment(Apartment apartment)
    {
        _apartmentRepository.Save(apartment);
    }

    public void AddApartmentWithValidation(Apartment apartment)
    {
        var building = _buildingRepository.GetById(apartment.BuildingId);
        if (building == null) throw new InvalidOperationException("Zgrada nije pronađena.");
        if (building.Status != BuildingStatus.Approved)
            throw new InvalidOperationException("Stanove možete dodavati samo u odobrene zgrade.");
        if (_apartmentRepository.ExistsApartmentNumberInBuilding(apartment.ApartmentNumber, apartment.BuildingId))
            throw new InvalidOperationException("Stan sa tim brojem već postoji u ovoj zgradi.");

        _apartmentRepository.Save(apartment);
    }
}
