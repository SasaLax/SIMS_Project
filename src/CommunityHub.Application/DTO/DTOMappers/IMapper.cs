namespace CommunityHub.Application.DTO.DTOMappers;

public interface IMapper<TFrom, TTo>
{
    TTo Map(TFrom source);
}
