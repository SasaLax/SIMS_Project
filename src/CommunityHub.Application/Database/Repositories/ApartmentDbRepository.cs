using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class ApartmentDbRepository : IApartmentRepository
{
    public void Save(Apartment apartment)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
                INSERT INTO apartments (apartment_number, description, number_of_rooms, max_number_of_residents, building_id)
                VALUES (@apartmentNumber, @description, @numberOfRooms, @maxNumberOfResidents, @buildingId)
                RETURNING id";

        AddParameter(command, "@apartmentNumber", apartment.ApartmentNumber);
        AddParameter(command, "@description", apartment.Description);
        AddParameter(command, "@numberOfRooms", apartment.NumberOfRooms);
        AddParameter(command, "@maxNumberOfResidents", apartment.MaxNumberOfResidents);
        AddParameter(command, "@buildingId", apartment.BuildingId);

        apartment.Id = Convert.ToInt32(command.ExecuteScalar());
    }

    public Apartment? GetById(long id)
    {

        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
                SELECT id, apartment_number, description, number_of_rooms, max_number_of_residents, building_id
                FROM apartments
                WHERE id = @id";

        AddParameter(command, "@id", id);

        using IDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return MapApartmentFromReader(reader);
        }

        return null;
    }

    public List<Apartment> GetByBuildingId(long buildingId)
    {
        List<Apartment> apartments = new List<Apartment>();


        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
                SELECT id, apartment_number, description, number_of_rooms, max_number_of_residents, building_id
                FROM apartments
                WHERE building_id = @buildingId";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            apartments.Add(MapApartmentFromReader(reader));
        }

        return apartments;
    }

    private Apartment MapApartmentFromReader(IDataReader reader)
    {
        long id = Convert.ToInt64(reader.GetValue(0));
        int apartmentNumber = Convert.ToInt32(reader.GetValue(1));
        string description = reader.GetString(2);
        int numberOfRooms = Convert.ToInt32(reader.GetValue(3));
        int maxNumberOfResidents = Convert.ToInt32(reader.GetValue(4));
        long buildingId = Convert.ToInt64(reader.GetValue(5));

        return new Apartment(id, apartmentNumber, description, numberOfRooms, maxNumberOfResidents, buildingId);
    }

    private void AddParameter(IDbCommand command, string name, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    public bool ExistsApartmentNumberInBuilding(int apartmentNumber, long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM apartments WHERE apartment_number = @apartmentNumber AND building_id = @buildingId LIMIT 1";

        AddParameter(command, "@apartmentNumber", apartmentNumber);
        AddParameter(command, "@buildingId", buildingId);

        object? result = command.ExecuteScalar();
        return result != null && result != DBNull.Value;
    }
}
