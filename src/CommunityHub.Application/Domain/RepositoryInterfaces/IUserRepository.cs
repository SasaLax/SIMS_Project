using System;
using System.Collections.Generic;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Domain.RepositoryInterfaces;

public interface IUserRepository
{
    long? GetIdByCredentials(string email, string password);
    User? GetById(long userId);
    User? GetByEmail(string email);
    bool EmailExists(string email);
    bool JmbgExists(string jmbg);
    bool PasswordExists(string password);
    long Create(User user);
    void Save(User user);
}
