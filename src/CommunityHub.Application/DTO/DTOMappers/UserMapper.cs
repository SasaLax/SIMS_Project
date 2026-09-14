using CommunityHub.Application.Domain;

namespace CommunityHub.Application.DTO.DTOMappers;

public class UserMapper : IMapper<User, UserDTO>
{
    public UserDTO Map(User source) => UserDTOMapper.ToDto(source);
}
