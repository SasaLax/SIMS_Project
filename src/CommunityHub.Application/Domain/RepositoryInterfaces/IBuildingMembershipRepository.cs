namespace CommunityHub.Application.Domain.RepositoryInterfaces;

public interface IBuildingMembershipRepository
{
    bool Exists(long buildingId, int apartmentNumber);
}
