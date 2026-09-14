using System;
using System.Collections.Generic;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;

namespace CommunityHub.Application.Service;

public interface IAdminService
{
    void RegisterManager(User manager);
    void AddBuilding(Building building);
}

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IBuildingRepository _buildingRepository;

    public AdminService(IUserRepository userRepository, IBuildingRepository buildingRepository)
    {
        _userRepository = userRepository;
        _buildingRepository = buildingRepository;
    }

    public void RegisterManager(User manager)
    {
        if (_userRepository.EmailExists(manager.Email))
            throw new InvalidOperationException("Email already exists");
        
        if (_userRepository.JmbgExists(manager.Jmbg))
            throw new InvalidOperationException("JMBG already exists");

        _userRepository.Save(manager);
    }

    public void AddBuilding(Building building)
    {
        _buildingRepository.Save(building);
    }
}
