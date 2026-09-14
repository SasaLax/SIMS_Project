using System;
using System.Data;
using CommunityHub.Application.Domain.RepositoryInterfaces;

namespace CommunityHub.Application.Database.Repositories;

public class BuildingMembershipDbRepository : IBuildingMembershipRepository
{
    public bool Exists(long buildingId, int apartmentNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 1 FROM requests
            WHERE building_id = @buildingId
              AND apartment_number = @apartmentNumber
              AND status = 'Approved'
            LIMIT 1";

        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@apartmentNumber", apartmentNumber);

        object? result = command.ExecuteScalar();
        return result != null && result != DBNull.Value;
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
