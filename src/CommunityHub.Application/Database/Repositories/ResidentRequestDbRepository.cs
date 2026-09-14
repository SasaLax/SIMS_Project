using System;
using System.Collections.Generic;
using System.Data;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.RepositoryInterfaces;

namespace CommunityHub.Application.Database.Repositories;

public class ResidentRequestDbRepository : IResidentRequestRepository
{
    public long Create(ResidentRequest request)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
                INSERT INTO requests (user_id, building_id, apartment_number, created_at, status)
                VALUES (@userId, @buildingId, @apartmentNumber, @createdAt, @status)
                RETURNING id";

        AddParameter(command, "@userId", request.UserId);
        AddParameter(command, "@buildingId", request.BuildingId);
        AddParameter(command, "@apartmentNumber", request.ApartmentNumber);
        AddParameter(command, "@createdAt", request.CreatedAt);
        AddParameter(command, "@status", request.Status.ToString());

        object? result = command.ExecuteScalar();
        if (result == null || result == DBNull.Value) throw new InvalidOperationException("Failed to insert request.");
        return Convert.ToInt64(result);
    }

    public ResidentRequest? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
                SELECT id, user_id, building_id, apartment_number, created_at, status, rejection_reason, handled_by, handled_at
                FROM requests
                WHERE id = @id";
        AddParameter(command, "@id", id);

        using IDataReader reader = command.ExecuteReader();
        if (!reader.Read()) return null;
        return MapRequestFromReader(reader);
    }

    public IEnumerable<ResidentRequest> GetByUser(long userId, string? statusFilter = null)
    {
        var list = new List<ResidentRequest>();
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        if (string.IsNullOrWhiteSpace(statusFilter))
        {
            command.CommandText = @"
                    SELECT id, user_id, building_id, apartment_number, created_at, status, rejection_reason, handled_by, handled_at
                    FROM requests
                    WHERE user_id = @userId
                    ORDER BY created_at DESC";
            AddParameter(command, "@userId", userId);
        }
        else
        {
            command.CommandText = @"
                    SELECT id, user_id, building_id, apartment_number, created_at, status, rejection_reason, handled_by, handled_at
                    FROM requests
                    WHERE user_id = @userId AND status = @status
                    ORDER BY created_at DESC";
            AddParameter(command, "@userId", userId);
            AddParameter(command, "@status", statusFilter);
        }

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read()) list.Add(MapRequestFromReader(reader));
        return list;
    }

    public IEnumerable<ResidentRequest> GetByBuilding(long buildingId, string? statusFilter = null)
    {
        var list = new List<ResidentRequest>();
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        if (string.IsNullOrWhiteSpace(statusFilter))
        {
            command.CommandText = @"
                    SELECT id, user_id, building_id, apartment_number, created_at, status, rejection_reason, handled_by, handled_at
                    FROM requests
                    WHERE building_id = @buildingId
                    ORDER BY created_at DESC";
            AddParameter(command, "@buildingId", buildingId);
        }
        else
        {
            command.CommandText = @"
                    SELECT id, user_id, building_id, apartment_number, created_at, status, rejection_reason, handled_by, handled_at
                    FROM requests
                    WHERE building_id = @buildingId AND status = @status
                    ORDER BY created_at DESC";
            AddParameter(command, "@buildingId", buildingId);
            AddParameter(command, "@status", statusFilter);
        }

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read()) list.Add(MapRequestFromReader(reader));
        return list;
    }

    public bool HasPendingRequest(long userId, long buildingId, int apartmentNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 1 FROM requests
            WHERE user_id = @userId
              AND building_id = @buildingId
              AND apartment_number = @apartmentNumber
              AND status = 'Pending'
            LIMIT 1";

        AddParameter(command, "@userId", userId);
        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@apartmentNumber", apartmentNumber);

        object? result = command.ExecuteScalar();
        return result != null && result != DBNull.Value;
    }

    public void Delete(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM requests WHERE id = @id";
        AddParameter(command, "@id", id);
        command.ExecuteNonQuery();
    }

    public void UpdateStatus(long id, RequestStatus status, long? handledBy = null, string? rejectionReason = null)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
                UPDATE requests
                SET status = @status,
                    rejection_reason = @rejectionReason,
                    handled_by = @handledBy,
                    handled_at = @handledAt
                WHERE id = @id";

        AddParameter(command, "@status", status.ToString());
        AddParameter(command, "@rejectionReason", (object?)rejectionReason ?? DBNull.Value);
        AddParameter(command, "@handledBy", (object?)handledBy ?? DBNull.Value);
        AddParameter(command, "@handledAt", DateTime.UtcNow);
        AddParameter(command, "@id", id);

        command.ExecuteNonQuery();
    }

    public void Update(ResidentRequest request)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
                UPDATE requests
                SET status = @status,
                    rejection_reason = @rejectionReason,
                    handled_by = @handledBy,
                    handled_at = @handledAt
                WHERE id = @id";

        AddParameter(command, "@status", request.Status.ToString());
        AddParameter(command, "@rejectionReason", (object?)request.RejectionReason ?? DBNull.Value);
        AddParameter(command, "@handledBy", (object?)request.HandledByManagerId ?? DBNull.Value);
        AddParameter(command, "@handledAt", (object?)request.HandledAt ?? DBNull.Value);
        AddParameter(command, "@id", request.Id);

        command.ExecuteNonQuery();
    }

    private ResidentRequest MapRequestFromReader(IDataReader reader)
    {
        long id = Convert.ToInt64(reader.GetValue(0));
        long userId = Convert.ToInt64(reader.GetValue(1));
        long buildingId = Convert.ToInt64(reader.GetValue(2));
        int apartmentNumber = reader.GetInt32(3);
        DateTime createdAt = reader.GetDateTime(4);
        string statusString = reader.GetString(5);
        string? rejectionReason = reader.IsDBNull(6) ? null : reader.GetString(6);
        long? handledBy = reader.IsDBNull(7) ? null : (long?)reader.GetInt64(7);
        DateTime? handledAt = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8);

        if (!Enum.TryParse(statusString, out RequestStatus status))
        {
            status = RequestStatus.Pending;
        }

        return new ResidentRequest(id, userId, buildingId, apartmentNumber, createdAt, status, rejectionReason, handledBy, handledAt);
    }

    private void AddParameter(IDbCommand command, string name, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}