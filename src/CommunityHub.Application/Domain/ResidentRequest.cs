using System;

namespace CommunityHub.Application.Domain
{
    public enum RequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class ResidentRequest
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public long BuildingId { get; private set; }
        public int ApartmentNumber { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public RequestStatus Status { get; private set; }
        public string? RejectionReason { get; private set; }
        public long? HandledByManagerId { get; private set; }
        public DateTime? HandledAt { get; private set; }

        public ResidentRequest(
            long id,
            long userId,
            long buildingId,
            int apartmentNumber,
            DateTime createdAt,
            RequestStatus status,
            string? rejectionReason = null,
            long? handledByManagerId = null,
            DateTime? handledAt = null)
        {
            if (apartmentNumber <= 0) throw new ArgumentOutOfRangeException(nameof(apartmentNumber), "Apartment number must be positive.");

            Id = id;
            UserId = userId;
            BuildingId = buildingId;
            ApartmentNumber = apartmentNumber;
            CreatedAt = createdAt;
            Status = status;
            RejectionReason = rejectionReason;
            HandledByManagerId = handledByManagerId;
            HandledAt = handledAt;
        }

        public static ResidentRequest CreatePending(long userId, long buildingId, int apartmentNumber)
        {
            return new ResidentRequest(
                id: 0,
                userId: userId,
                buildingId: buildingId,
                apartmentNumber: apartmentNumber,
                createdAt: DateTime.UtcNow,
                status: RequestStatus.Pending);
        }

        public void Approve(long managerId)
        {
            if (Status != RequestStatus.Pending) throw new InvalidOperationException("Only pending requests can be approved.");
            Status = RequestStatus.Approved;
            RejectionReason = null;
            HandledByManagerId = managerId;
            HandledAt = DateTime.UtcNow;
        }

        public void Reject(long managerId, string reason)
        {
            if (Status != RequestStatus.Pending) throw new InvalidOperationException("Only pending requests can be rejected.");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Rejection reason is required.", nameof(reason));
            Status = RequestStatus.Rejected;
            RejectionReason = reason;
            HandledByManagerId = managerId;
            HandledAt = DateTime.UtcNow;
        }
    }
}