using Microsoft.Data.SqlClient;
using System.Data;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Services;

public class EmployeeService
{
    private readonly DatabaseConnection _databaseConnection;

    public EmployeeService(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<List<Employee>> GetEmployeesAsync(
        string? searchText = null)
    {
        var employees = new List<Employee>();

        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
            SELECT
                Id,
                Name,
                Department,
                IsActive
            FROM Employee
            WHERE
                @SearchText IS NULL
                OR Name LIKE N'%' + @SearchText + N'%'
                OR Department LIKE N'%' + @SearchText + N'%'
            ORDER BY Name ASC;";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@SearchText",
            SqlDbType.NVarChar,
            100
        ).Value =
            string.IsNullOrWhiteSpace(searchText)
                ? DBNull.Value
                : searchText;

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            employees.Add(
                new Employee
                {
                    Id = reader.GetInt32(
                        reader.GetOrdinal("Id")
                    ),

                    Name = reader.GetString(
                        reader.GetOrdinal("Name")
                    ),

                    Department =
                        reader.IsDBNull(
                            reader.GetOrdinal("Department")
                        )
                            ? null
                            : reader.GetString(
                                reader.GetOrdinal("Department")
                            ),

                    IsActive = reader.GetBoolean(
                        reader.GetOrdinal("IsActive")
                    )
                }
            );
        }

        return employees;
    }


    public async Task AddEmployeeAsync(
        Employee employee)
    {
        if (string.IsNullOrWhiteSpace(employee.Name))
        {
            throw new Exception(
                "Employee name оруулна уу."
            );
        }

        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
            INSERT INTO Employee
            (
                Name,
                Department,
                IsActive
            )
            VALUES
            (
                @Name,
                @Department,
                @IsActive
            );";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@Name",
            SqlDbType.NVarChar,
            100
        ).Value = employee.Name.Trim();

        command.Parameters.Add(
            "@Department",
            SqlDbType.NVarChar,
            100
        ).Value =
            string.IsNullOrWhiteSpace(employee.Department)
                ? DBNull.Value
                : employee.Department.Trim();

        command.Parameters.Add(
            "@IsActive",
            SqlDbType.Bit
        ).Value = employee.IsActive;

        await command.ExecuteNonQueryAsync();
    }


    public async Task UpdateEmployeeAsync(
        Employee employee)
    {
        if (string.IsNullOrWhiteSpace(employee.Name))
        {
            throw new Exception(
                "Employee name оруулна уу."
            );
        }

        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
            UPDATE Employee
            SET
                Name = @Name,
                Department = @Department,
                IsActive = @IsActive
            WHERE Id = @Id;";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@Id",
            SqlDbType.Int
        ).Value = employee.Id;

        command.Parameters.Add(
            "@Name",
            SqlDbType.NVarChar,
            100
        ).Value = employee.Name.Trim();

        command.Parameters.Add(
            "@Department",
            SqlDbType.NVarChar,
            100
        ).Value =
            string.IsNullOrWhiteSpace(employee.Department)
                ? DBNull.Value
                : employee.Department.Trim();

        command.Parameters.Add(
            "@IsActive",
            SqlDbType.Bit
        ).Value = employee.IsActive;

        await command.ExecuteNonQueryAsync();
    }


    public async Task DeactivateEmployeeAsync(
        int id)
    {
        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
            UPDATE Employee
            SET IsActive = 0
            WHERE Id = @Id;";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@Id",
            SqlDbType.Int
        ).Value = id;

        await command.ExecuteNonQueryAsync();
    }


    public async Task DeleteEmployeeAsync(
        int id)
    {
        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string checkSql = @"
            SELECT
                IsActive,
                (
                    SELECT COUNT(*)
                    FROM [Task]
                    WHERE EmployeeId = @Id
                ) AS TaskCount
            FROM Employee
            WHERE Id = @Id;";

        using SqlCommand checkCommand =
            new SqlCommand(checkSql, connection);

        checkCommand.Parameters.Add(
            "@Id",
            SqlDbType.Int
        ).Value = id;

        bool isActive;
        int taskCount;

        using (SqlDataReader reader =
            await checkCommand.ExecuteReaderAsync())
        {
            if (!await reader.ReadAsync())
            {
                throw new Exception(
                    "Employee олдсонгүй."
                );
            }

            isActive = reader.GetBoolean(
                reader.GetOrdinal("IsActive")
            );

            taskCount = reader.GetInt32(
                reader.GetOrdinal("TaskCount")
            );
        }

        if (isActive)
        {
            throw new Exception(
                "Active Employee-г устгах боломжгүй. Эхлээд Inactive болгоно уу."
            );
        }

        if (taskCount > 0)
        {
            throw new Exception(
                "Энэ Employee дээр Task бүртгэлтэй байгаа тул устгах боломжгүй."
            );
        }

        string deleteSql = @"
            DELETE FROM Employee
            WHERE Id = @Id
              AND IsActive = 0;";

        using SqlCommand deleteCommand =
            new SqlCommand(deleteSql, connection);

        deleteCommand.Parameters.Add(
            "@Id",
            SqlDbType.Int
        ).Value = id;

        await deleteCommand.ExecuteNonQueryAsync();
    }
}
