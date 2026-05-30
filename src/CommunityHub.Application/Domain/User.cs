using System;

namespace CommunityHub.Application.Domain;

public enum UserRole
{
    Administrator,
    Resident,
    Manager
}

public class User
{
    public long Id { get; private set; }
    public string Jmbg { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string PhoneNumber { get; private set; }
    public UserRole Role { get; private set; }

    public User(long id, string jmbg, string email, string password, string name, string surname, string phoneNumber, UserRole role)
    {
        Id = id;
        Jmbg = jmbg;
        Email = email;
        Password = password;
        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;
        Role = role;
    }
}