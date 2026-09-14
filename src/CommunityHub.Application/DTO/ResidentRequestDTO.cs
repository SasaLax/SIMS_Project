namespace CommunityHub.Application.DTO;

public class ResidentRequestDTO
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long BuildingId { get; set; }
    public int ApartmentNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public long? HandledByManagerId { get; set; }
    public DateTime? HandledAt { get; set; }
    public string? UserFullName { get; set; }
    public string? BuildingAddress { get; set; }
}
