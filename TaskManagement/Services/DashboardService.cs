using Microsoft.Data.SqlClient;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Services;

public class DashboardService
{
    private readonly DatabaseConnection _databaseConnection;

    public DashboardService(
        DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
            SELECT
                COUNT(*) AS TotalTask,

                SUM(
                    CASE
                        WHEN Status = 1 THEN 1
                        ELSE 0
                    END
                ) AS NewTask,

                SUM(
                    CASE
                        WHEN Status = 2 THEN 1
                        ELSE 0
                    END
                ) AS InProgressTask,

                SUM(
                    CASE
                        WHEN Status = 3 THEN 1
                        ELSE 0
                    END
                ) AS CompletedTask,

                SUM(
                    CASE
                        WHEN DueDate < CAST(GETDATE() AS DATE)
                             AND Status <> 3
                        THEN 1
                        ELSE 0
                    END
                ) AS OverdueTask

            FROM [Task];";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return new DashboardStats();
        }

        return new DashboardStats
        {
            TotalTask =
                Convert.ToInt32(reader["TotalTask"]),

            NewTask =
                Convert.ToInt32(reader["NewTask"]),

            InProgressTask =
                Convert.ToInt32(reader["InProgressTask"]),

            CompletedTask =
                Convert.ToInt32(reader["CompletedTask"]),

            OverdueTask =
                Convert.ToInt32(reader["OverdueTask"])
        };
    }
}