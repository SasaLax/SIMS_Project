using System;
using System.Text.RegularExpressions;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;

namespace CommunityHub.Application.Service;

public interface IUserService
{
    long RegisterResident(User resident);
    User? Login(string email, string password);
    User? GetById(long userId);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public long RegisterResident(User resident)
    {
        if (resident == null) throw new ArgumentNullException(nameof(resident));

        if (resident.Role != UserRole.Resident)
            throw new InvalidOperationException("Ova metoda je namenjena isključivo za registraciju stanara.");

        if (string.IsNullOrWhiteSpace(resident.Jmbg) || !Regex.IsMatch(resident.Jmbg, @"^\d{13}$"))
            throw new InvalidOperationException("JMBG mora sadržati tačno 13 cifara.");

        if (string.IsNullOrWhiteSpace(resident.Email) || !resident.Email.Contains("@"))
            throw new InvalidOperationException("Unesite validan email.");

        if (string.IsNullOrWhiteSpace(resident.Password) || resident.Password.Length < 6)
            throw new InvalidOperationException("Lozinka mora imati najmanje 6 znakova.");

        if (string.IsNullOrWhiteSpace(resident.PhoneNumber) || !Regex.IsMatch(resident.PhoneNumber, @"^\+387\d{8}$"))
            throw new InvalidOperationException("Mobilni broj mora početi sa +387 i imati 8 cifara nakon toga (npr. +38765111222).");

        if (_userRepository.EmailExists(resident.Email))
            throw new InvalidOperationException("Email već postoji.");

        if (_userRepository.JmbgExists(resident.Jmbg))
            throw new InvalidOperationException("Korisnik sa datim JMBG već postoji.");

        if (_userRepository.PasswordExists(resident.Password))
            throw new InvalidOperationException("Lozinka mora biti jedinstvena. Izaberite drugu lozinku.");

        return _userRepository.Create(resident);
    }

    public User? Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Molimo unesite email i lozinku.");

        long? userId = _userRepository.GetIdByCredentials(email, password);
        if (userId == null)
            throw new InvalidOperationException("Pogrešan email ili lozinka!");

        User? user = _userRepository.GetById(userId.Value);
        if (user == null)
            throw new InvalidOperationException("Greška pri učitavanju profila korisnika.");

        return user;
    }

    public User? GetById(long userId) => _userRepository.GetById(userId);
}
