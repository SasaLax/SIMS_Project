using CommunityHub.Application.Domain;

namespace CommunityHub.Application.DTO.DTOMappers;

public static class ResidentRequestDTOMapper
{
    public static ResidentRequestDTO ToDto(ResidentRequest request)
    {
        return new ResidentRequestDTO
        {
            Id = request.Id,
            UserId = request.UserId,
            BuildingId = request.BuildingId,
            ApartmentNumber = request.ApartmentNumber,
            CreatedAt = request.CreatedAt,
            Status = request.Status.ToString(),
            RejectionReason = request.RejectionReason,
            HandledByManagerId = request.HandledByManagerId,
            HandledAt = request.HandledAt
        };
    }
}
