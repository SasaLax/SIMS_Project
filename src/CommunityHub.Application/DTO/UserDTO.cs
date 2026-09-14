namespace CommunityHub.Application.DTO;

public class UserDTO
{
    public long Id { get; set; }
    public string Jmbg { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string FullName => $"{Name} {Surname}";
}
