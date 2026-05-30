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

        public List<Building> GetAll(bool sortByFloors = false)
        {
            List<Building> buildings = new List<Building>();
            using IDbConnection connection = PostgresConnection.CreateConnection();
            using IDbCommand command = connection.CreateCommand();

            string query = @"
                SELECT b.id, a.street, a.number, b.neighbourhood, l.city, l.country, b.number_of_floors, b.manager_jmbg
                FROM buildings b
                JOIN addresses a ON b.address_id = a.id
                JOIN locations l ON b.location_id = l.id";

            if (sortByFloors)
            {
                query += " ORDER BY b.number_of_floors ASC";
            }

            command.CommandText = query;
            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                buildings.Add(MapBuildingFromReader(reader));
            }
            return buildings;
        }

        public List<Building> SearchByAddress(string queryText)
        {
            return ExecuteSearchQuery(@"
                SELECT b.id, a.street, a.number, b.neighbourhood, l.city, l.country, b.number_of_floors, b.manager_jmbg
                FROM buildings b
                JOIN addresses a ON b.address_id = a.id
                JOIN locations l ON b.location_id = l.id
                WHERE a.street ILIKE @query OR CAST(a.number AS TEXT) ILIKE @query", $"%{queryText}%");
        }

        public List<Building> SearchByNeighbourhood(string queryText)
        {
            return ExecuteSearchQuery(@"
                SELECT b.id, a.street, a.number, b.neighbourhood, l.city, l.country, b.number_of_floors, b.manager_jmbg
                FROM buildings b
                JOIN addresses a ON b.address_id = a.id
                JOIN locations l ON b.location_id = l.id
                WHERE b.neighbourhood ILIKE @query", $"%{queryText}%");
        }

        public List<Building> SearchByFloors(int floors)
        {
            List<Building> buildings = new List<Building>();
            using IDbConnection connection = PostgresConnection.CreateConnection();
            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT b.id, a.street, a.number, b.neighbourhood, l.city, l.country, b.number_of_floors, b.manager_jmbg
                FROM buildings b
                JOIN addresses a ON b.address_id = a.id
                JOIN locations l ON b.location_id = l.id
                WHERE b.number_of_floors = @floors";

            AddParameter(command, "@floors", floors);
            using IDataReader reader = command.ExecuteReader();
            while (reader.Read()) buildings.Add(MapBuildingFromReader(reader));
            return buildings;
        }

        public List<Building> SearchByApartmentsAdvanced(int rooms, int residents, string op)
        {
            List<Building> buildings = new List<Building>();
            using IDbConnection connection = PostgresConnection.CreateConnection();
            using IDbCommand command = connection.CreateCommand();

            // Osnovni upit sa DISTINCT da nam ne duplira istu zgradu ako ima više stanova koji odgovaraju
            string sql = @"
                SELECT DISTINCT b.id, a.street, a.number, b.neighbourhood, l.city, l.country, b.number_of_floors, b.manager_jmbg
                FROM buildings b
                JOIN addresses a ON b.address_id = a.id
                JOIN locations l ON b.location_id = l.id
                JOIN apartments ap ON b.id = ap.building_id
                WHERE ";

            if (op == "&") // logičko I ima toliko soba i stanara
            {
                sql += "ap.number_of_rooms = @rooms AND ap.max_number_of_residents = @residents";
            }
            else if (op == "|") //soba ili stanara
            {
                sql += "ap.number_of_rooms = @rooms OR ap.max_number_of_residents = @residents";
            }
            else if (rooms > 0 && residents == 0) // po broju soba
            {
                sql += "ap.number_of_rooms = @rooms";
            }
            else // po broju stanara
            {
                sql += "ap.max_number_of_residents = @residents";
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


        public void AddParameter(IDbCommand command, string name, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}   
