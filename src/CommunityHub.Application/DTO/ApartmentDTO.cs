namespace CommunityHub.Application.DTO;

public class ApartmentDTO
{
    public long Id { get; set; }
    public int ApartmentNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public int NumberOfRooms { get; set; }
    public int MaxNumberOfResidents { get; set; }
    public long BuildingId { get; set; }
}
