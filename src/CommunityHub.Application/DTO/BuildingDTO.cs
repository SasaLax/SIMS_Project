namespace CommunityHub.Application.DTO;

public class BuildingDTO
{
    public long Id { get; set; }
    public string BuildingCode { get; set; } = string.Empty;   
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Neighbourhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int NumberOfFloors { get; set; }
    public string? ManagerJmbg { get; set; }
    public string Status { get; set; } = string.Empty;
    public string FullAddress => $"{Street} {Number}";
    public string FullLocation => $"{City}, {Country}";
}
