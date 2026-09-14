using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;
using CommunityHub.Application.DTO;
using CommunityHub.Application.DTO.DTOMappers;
using CommunityHub.Application.Service;
using System;
using System.Collections.Generic;

namespace CommunityHub.Application;

public class Injector
{
    private static Dictionary<Type, object> _implementations = InitializeDefaults();

    private static Dictionary<Type, object> InitializeDefaults()
    {
        var userRepo = new UserDbRepository();
        var buildingRepo = new BuildingDbRepository();
        var apartmentRepo = new ApartmentDbRepository();
        var requestRepo = new ResidentRequestDbRepository();
        var membershipRepo = new BuildingMembershipDbRepository();

        return new Dictionary<Type, object>
        {
            { typeof(IUserRepository), userRepo },
            { typeof(IBuildingRepository), buildingRepo },
            { typeof(IApartmentRepository), apartmentRepo },
            { typeof(IResidentRequestRepository), requestRepo },
            { typeof(IBuildingMembershipRepository), membershipRepo },

            { typeof(IUserService), new UserService(userRepo) },
            { typeof(IBuildingService), new BuildingService(buildingRepo, apartmentRepo) },
            {
                typeof(IBuildingAccessRequestService),
                new BuildingAccessRequestService(requestRepo, apartmentRepo, membershipRepo)
            },
            {
                typeof(IManagerService),
                new ManagerService(buildingRepo, requestRepo, apartmentRepo)
            },
            {
                typeof(IAdminService),
                new AdminService(userRepo, buildingRepo)
            },

            { typeof(IMapper<User, UserDTO>), new UserMapper() },
            { typeof(IMapper<Building, BuildingDTO>), new BuildingMapper() },
            { typeof(IMapper<Apartment, ApartmentDTO>), new ApartmentMapper() },
            { typeof(IMapper<ResidentRequest, ResidentRequestDTO>), new ResidentRequestMapper() }
        };
    }

    public static T CreateInstance<T>()
    {
        Type type = typeof(T);
        if (_implementations.ContainsKey(type))
            return (T)_implementations[type];
        throw new ArgumentException($"No implementation found for {type}");
    }

    public static void Register<TInterface>(TInterface implementation)
    {
        if (implementation == null) throw new ArgumentNullException(nameof(implementation));
        _implementations[typeof(TInterface)] = implementation;
    }

    public static void Override<TInterface>(TInterface implementation)
        => Register(implementation);

    public static void ResetToDefaults()
    {
        _implementations = InitializeDefaults();
    }
}

