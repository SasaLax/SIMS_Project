using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;

namespace CommunityHub.Application.Database.Repositories;

public class BuildingDbRepository : IBuildingRepository
{
    // BAZNI UPIT: Ako se ikada doda nova kolona, mijenja se SAMO OVDJE!
    private const string BaseSelectSql = @"
        SELECT b.id, b.building_code, a.street, a.number, b.neighbourhood, 
               l.city, l.country, b.number_of_floors, b.manager_jmbg, b.status
        FROM buildings b
        JOIN addresses a ON b.address_id = a.id
        JOIN locations l ON b.location_id = l.id";

    public void Save(Building building)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        long addressId = GetOrCreateAddress(connection, building.Address);
        long locationId = GetOrCreateLocation(connection, building.Location);

        using IDbCommand command = connection.CreateCommand();

        // Ako je Id > 0 upisujemo ga, ako je 0 (nova zgrada) puštamo bazu da sama dodijeli ID
        if (building.Id > 0)
        {
            command.CommandText = @"
                INSERT INTO buildings (id, building_code, address_id, neighbourhood, location_id, number_of_floors, manager_jmbg, status)
                VALUES (@id, @buildingCode, @addressId, @neighbourhood, @locationId, @numberOfFloors, @managerJmbg, @status)";
            AddParameter(command, "@id", building.Id);
        }
        else
        {
            command.CommandText = @"
                INSERT INTO buildings (building_code, address_id, neighbourhood, location_id, number_of_floors, manager_jmbg, status)
                VALUES (@buildingCode, @addressId, @neighbourhood, @locationId, @numberOfFloors, @managerJmbg, @status)";
        }

        AddParameter(command, "@buildingCode", building.BuildingCode);
        AddParameter(command, "@addressId", addressId);
        AddParameter(command, "@neighbourhood", building.Neighbourhood);
        AddParameter(command, "@locationId", locationId);
        AddParameter(command, "@numberOfFloors", building.NumberOfFloors);
        AddParameter(command, "@managerJmbg", (object?)building.ManagerJmbg ?? DBNull.Value);
        AddParameter(command, "@status", building.Status.ToString());

        command.ExecuteNonQuery();
    }

    public Building? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = $"{BaseSelectSql} WHERE b.id = @id";
        AddParameter(command, "@id", id);

        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapBuildingFromReader(reader);
        }

        return null;
    }

    public List<Building> GetAll(bool sortByFloors = false, bool onlyApproved = false)
    {
        List<Building> buildings = new List<Building>();
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        string sql = BaseSelectSql;

        if (onlyApproved)
        {
            sql += " WHERE b.status = 'Approved'";
        }

        if (sortByFloors)
        {
            sql += " ORDER BY b.number_of_floors ASC";
        }

        command.CommandText = sql;
        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            buildings.Add(MapBuildingFromReader(reader));
        }
        return buildings;
    }

    public List<Building> GetByManagerJmbg(string managerJmbg)
    {
        List<Building> buildings = new List<Building>();
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = $"{BaseSelectSql} WHERE b.manager_jmbg = @managerJmbg";
        AddParameter(command, "@managerJmbg", managerJmbg);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            buildings.Add(MapBuildingFromReader(reader));
        }
        return buildings;
    }

    public List<Building> SearchByAddress(string queryText)
    {
        return ExecuteSearchQuery(
            $"{BaseSelectSql} WHERE (a.street ILIKE @query OR CAST(a.number AS TEXT) ILIKE @query) AND b.status = 'Approved'",
            $"%{queryText}%"
        );
    }

    public List<Building> SearchByNeighbourhood(string queryText)
    {
        return ExecuteSearchQuery(
            $"{BaseSelectSql} WHERE b.neighbourhood ILIKE @query AND b.status = 'Approved'",
            $"%{queryText}%"
        );
    }

    public List<Building> SearchByFloors(int floors)
    {
        List<Building> buildings = new List<Building>();
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = $"{BaseSelectSql} WHERE b.number_of_floors = @floors AND b.status = 'Approved'";
        AddParameter(command, "@floors", floors);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            buildings.Add(MapBuildingFromReader(reader));
        }
        return buildings;
    }

    public List<Building> SearchByApartmentsAdvanced(int rooms, int residents, string op)
    {
        if (rooms <= 0 && residents <= 0)
        {
            return new List<Building>();
        }

        List<Building> buildings = new List<Building>();
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        string sql = @"
                SELECT DISTINCT b.id, b.building_code, a.street, a.number, b.neighbourhood, 
                                l.city, l.country, b.number_of_floors, b.manager_jmbg, b.status
                FROM buildings b
                JOIN addresses a ON b.address_id = a.id
                JOIN locations l ON b.location_id = l.id
                JOIN apartments ap ON b.id = ap.building_id
                WHERE b.status = 'Approved'";

        if (rooms > 0 && residents > 0)
        {
            if (op == "&")
            {
                sql += " AND ap.number_of_rooms = @rooms AND ap.max_number_of_residents = @residents";
            }
            else if (op == "|")
            {
                sql += " AND (ap.number_of_rooms = @rooms OR ap.max_number_of_residents = @residents)";
            }
        }
        else if (rooms > 0)
        {
            sql += " AND ap.number_of_rooms = @rooms";
        }
        else if (residents > 0)
        {
            sql += " AND ap.max_number_of_residents = @residents";
        }

        command.CommandText = sql;
        AddParameter(command, "@rooms", rooms);
        AddParameter(command, "@residents", residents);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            buildings.Add(MapBuildingFromReader(reader));
        }
        return buildings;
    }

    public void Update(Building building)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        long addressId = GetOrCreateAddress(connection, building.Address);
        long locationId = GetOrCreateLocation(connection, building.Location);

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
                UPDATE buildings
                SET building_code = @buildingCode,
                    address_id = @addressId, 
                    neighbourhood = @neighbourhood, 
                    location_id = @locationId,
                    number_of_floors = @numberOfFloors, 
                    manager_jmbg = @managerJmbg, 
                    status = @status
                WHERE id = @id";

        AddParameter(command, "@id", building.Id);
        AddParameter(command, "@buildingCode", building.BuildingCode);
        AddParameter(command, "@addressId", addressId);
        AddParameter(command, "@neighbourhood", building.Neighbourhood);
        AddParameter(command, "@locationId", locationId);
        AddParameter(command, "@numberOfFloors", building.NumberOfFloors);
        AddParameter(command, "@managerJmbg", (object?)building.ManagerJmbg ?? DBNull.Value);
        AddParameter(command, "@status", building.Status.ToString());

        command.ExecuteNonQuery();
    }

    private List<Building> ExecuteSearchQuery(string sql, string paramValue)
    {
        List<Building> buildings = new List<Building>();
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameter(command, "@query", paramValue);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            buildings.Add(MapBuildingFromReader(reader));
        }
        return buildings;
    }

    private Building MapBuildingFromReader(IDataReader reader)
    {
        long id = Convert.ToInt64(reader["id"]);
        string buildingCode = reader["building_code"].ToString()!;
        string street = reader["street"].ToString()!;
        int number = Convert.ToInt32(reader["number"]);
        string neighbourhood = reader["neighbourhood"].ToString()!;
        string city = reader["city"].ToString()!;
        string country = reader["country"].ToString()!;
        int numberOfFloors = Convert.ToInt32(reader["number_of_floors"]);
        string? managerJmbg = reader["manager_jmbg"] is DBNull ? null : reader["manager_jmbg"].ToString();

        string statusStr = reader["status"].ToString()!;
        if (!Enum.TryParse<BuildingStatus>(statusStr, out var status))
        {
            status = BuildingStatus.Pending;
        }

        Address address = new Address(street, number);
        Location location = new Location(city, country);

        return new Building(id, buildingCode, address, neighbourhood, location, numberOfFloors, managerJmbg, status);
    }

    private long GetOrCreateAddress(IDbConnection connection, Address address)
    {
        using (IDbCommand checkCmd = connection.CreateCommand())
        {
            checkCmd.CommandText = "SELECT id FROM addresses WHERE street = @street AND number = @number";
            AddParameter(checkCmd, "@street", address.Street);
            AddParameter(checkCmd, "@number", address.Number);

            object? result = checkCmd.ExecuteScalar();
            if (result != null)
            {
                return Convert.ToInt64(result);
            }
        }

        using (IDbCommand insertCmd = connection.CreateCommand())
        {
            insertCmd.CommandText = "INSERT INTO addresses (street, number) VALUES (@street, @number) RETURNING id";
            AddParameter(insertCmd, "@street", address.Street);
            AddParameter(insertCmd, "@number", address.Number);
            return Convert.ToInt64(insertCmd.ExecuteScalar());
        }
    }

    private long GetOrCreateLocation(IDbConnection connection, Location location)
    {
        using (IDbCommand checkCmd = connection.CreateCommand())
        {
            checkCmd.CommandText = "SELECT id FROM locations WHERE city = @city AND country = @country";
            AddParameter(checkCmd, "@city", location.City);
            AddParameter(checkCmd, "@country", location.Country);

            object? result = checkCmd.ExecuteScalar();
            if (result != null)
            {
                return Convert.ToInt64(result);
            }
        }

        using (IDbCommand insertCmd = connection.CreateCommand())
        {
            insertCmd.CommandText = "INSERT INTO locations (city, country) VALUES (@city, @country) RETURNING id";
            AddParameter(insertCmd, "@city", location.City);
            AddParameter(insertCmd, "@country", location.Country);
            return Convert.ToInt64(insertCmd.ExecuteScalar());
        }
    }

    public void AddParameter(IDbCommand command, string name, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    public bool ExistsApartmentNumberInBuilding(long apartmentId, long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM apartments WHERE id = @id AND building_id = @buildingId LIMIT 1";

        AddParameter(command, "@id", apartmentId);
        AddParameter(command, "@buildingId", buildingId);

        object? result = command.ExecuteScalar();
        return result != null && result != DBNull.Value;
    }
}