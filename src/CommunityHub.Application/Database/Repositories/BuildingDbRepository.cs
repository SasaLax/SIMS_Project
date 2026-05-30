using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Database.Repositories
{
    public class BuildingDbRepository
    {
        public void Save(Building building)
        {
            using IDbConnection connection = PostgresConnection.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO buildings (id, street, number, neighboorhood, city, country, number_of_floors, manager)
                VALUES (@id, @street, @number, @neighboorhood, @city, @country, @numberOfFloors, @manager)";

            AddParameter(command, "@id", building.id);
            AddParameter(command, "@street", building.Address.Street);
            AddParameter(command, "@number", building.Address.Number);
            AddParameter(command, "@neighboorhood", building.Neighbourhood);
            AddParameter(command, "@city", building.Location.City);
            AddParameter(command, "@country", building.Location.Country);
            AddParameter(command, "@numberOfFloors", building.numberOfFloors);
            AddParameter(command, "@manager", (object?)building.Manager ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public Building? getById(string Id)
        {
            using IDbConnection connection = PostgresConnection.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM buildings WHERE id = @id";

            AddParameter(command, "@id", Id);

            using IDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapBuildingFromReader(reader);
            }

            return null;
        }

        private Building MapBuildingFromReader(IDataReader reader)
        {
            string id = reader.GetString(0);
            string street = reader.GetString(1);
            int number = Convert.ToInt32(reader.GetValue(2));
            string neightboorhood = reader.GetString(3);
            string city = reader.GetString(4);
            string country = reader.GetString(5);
            int numberOfFloors = reader.GetInt32(6);

            string? manager = reader.IsDBNull(7) ? null : reader.GetString(7);

            Address address = new Address(street, number);
            Location location = new Location(city, country);

            return new Building(id, address, neightboorhood, location, numberOfFloors, manager);
        }

        public List<Building> GetAll()
        {
            List<Building> buildings = new List<Building>();

            using IDbConnection connection = PostgresConnection.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
            SELECT id, street, number, neighbourhood, city, country, number_of_floors, manager
            FROM buildings";

            using IDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                buildings.Add(MapBuildingFromReader(reader));
            }

            return buildings;
        }



        public void AddParameter(IDbCommand command, string name, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}   
