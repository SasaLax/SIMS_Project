using CommunityHub.Application.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CommunityHub.Application.Database.Repositories
{
    public class ApartmentDbRepository
    {
        public void Save(Apartment apartment)
        {
            using IDbConnection connection = PostgresConnection.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO apartments (id, description, number_of_rooms, max_number_of_residents, building_id)
            VALUES (@id, @description, @numberOfRooms, @maxNumberOfResidents, @buildingId)";

            AddParameter(command, "@id", apartment.Id);
            AddParameter(command, "@description", apartment.Description);
            AddParameter(command, "@numberOfRooms", apartment.numberOfRooms);
            AddParameter(command, "@maxNumberOfResidents", apartment.maxNumberOfResidents);
            AddParameter(command, "@buildingId", apartment.buildingId);

            command.ExecuteNonQuery();
        }

        public Apartment? GetById(int id)
        {
            using IDbConnection connection = PostgresConnection.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
            SELECT id, description, number_of_rooms, max_number_of_residents, building_id
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

        public List<Apartment> GetByBuildingId(string buildingId)
        {
            List<Apartment> apartments = new List<Apartment>();

            using IDbConnection connection = PostgresConnection.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
            SELECT id, description, number_of_rooms, max_number_of_residents, building_id
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
            int id = Convert.ToInt32(reader.GetValue(0));
            string description = reader.GetString(1);
            int numberOfRooms = Convert.ToInt32(reader.GetValue(2));
            int maxNumberOfResidents = Convert.ToInt32(reader.GetValue(3));
            string buildingId = reader.GetString(4);

            return new Apartment(id, description, numberOfRooms, maxNumberOfResidents, buildingId);
        }

        private void AddParameter(IDbCommand command, string name, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}
