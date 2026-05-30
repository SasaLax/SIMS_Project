using System;
using System.Collections.Generic;
using System.Data;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Database.Repositories
{
    public class UserDbRepository
    {
       
        public long? GetIdByCredentials(string email, string password)
        {
            using IDbConnection connection = PostgresConnection.CreateConnection();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT id FROM users WHERE email = @email AND password = @password";

            AddParameter(command, "@email", email);
            AddParameter(command, "@password", password);

            object? result = command.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt64(result);
            }

            return null;
        }

        public User? GetById(long userId)
        {
            using IDbConnection connection = PostgresConnection.CreateConnection();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT id, jmbg, email, password, name, surname, phone_number, user_type FROM users WHERE id = @userId";

            AddParameter(command, "@userId", userId);

            using IDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapUserFromReader(reader);
            }

            return null;
        }

        
        public void Save(User user)
        {
            using IDbConnection connection = PostgresConnection.CreateConnection();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO users (jmbg, email, password, name, surname, phone_number, user_type)
                VALUES (@jmbg, @email, @password, @name, @surname, @phoneNumber, @userType)";

            AddParameter(command, "@jmbg", user.Jmbg);
            AddParameter(command, "@email", user.Email);
            AddParameter(command, "@password", user.Password);
            AddParameter(command, "@name", user.Name);
            AddParameter(command, "@surname", user.Surname);
            AddParameter(command, "@phoneNumber", user.PhoneNumber);
           
            AddParameter(command, "@userType", user.Role.ToString());

            command.ExecuteNonQuery();
        }

        
        private User MapUserFromReader(IDataReader reader)
        {
            long id = Convert.ToInt64(reader.GetValue(0));
            string jmbg = reader.GetString(1);
            string email = reader.GetString(2);
            string password = reader.GetString(3);
            string name = reader.GetString(4);
            string surname = reader.GetString(5);
            string phoneNumber = reader.GetString(6);
            string roleString = reader.GetString(7);

            //teskst iz baze nazad u c#
            if (!Enum.TryParse(roleString, out UserRole role))
            {
                role = UserRole.Resident; // default je resident
            }

            return new User(id, jmbg, email, password, name, surname, phoneNumber, role);
        }

        //sprecava sqlinjection
        public void AddParameter(IDbCommand command, string name, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}