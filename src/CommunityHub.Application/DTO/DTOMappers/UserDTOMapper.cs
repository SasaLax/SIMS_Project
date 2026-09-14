using CommunityHub.Application.Domain;

namespace CommunityHub.Application.DTO.DTOMappers;

public static class UserDTOMapper
{
    public static UserDTO ToDto(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Jmbg = user.Jmbg,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString()
        };
    }
}
