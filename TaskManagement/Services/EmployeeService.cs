using Microsoft.Data.SqlClient;
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

    public async Task DeactivateEmployeeAsync(int id)
    {
        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
        UPDATE Employee
        SET IsActive = 0
        WHERE Id = @id;";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@id",
            System.Data.SqlDbType.Int
        ).Value = id;

        await command.ExecuteNonQueryAsync();
    }
    public async Task<List<Employee>> GetEmployeesAsync()
    {
        List<Employee> employees = new();

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
            ORDER BY Name;";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Employee employee = new()
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
            };

            employees.Add(employee);
        }

        return employees;
    }

    public async Task<List<Employee>> GetEmployeesAsync(string? searchText = null)
    {
        List<Employee> employees = new();

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
            @searchText IS NULL
            OR Name LIKE N'%' + @searchText + N'%'
            OR Department LIKE N'%' + @searchText + N'%'
        ORDER BY Name;";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@searchText",
            System.Data.SqlDbType.NVarChar,
            100
        ).Value =
            string.IsNullOrWhiteSpace(searchText)
                ? DBNull.Value
                : searchText;

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Employee employee = new()
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
            };

            employees.Add(employee);
        }

        return employees;
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
        UPDATE Employee
        SET
            Name = @name,
            Department = @department,
            IsActive = @isActive
        WHERE Id = @id;";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@name",
            System.Data.SqlDbType.NVarChar,
            100
        ).Value = employee.Name;

        command.Parameters.Add(
            "@department",
            System.Data.SqlDbType.NVarChar,
            100
        ).Value = (object?)employee.Department
            ?? DBNull.Value;

        command.Parameters.Add(
            "@isActive",
            System.Data.SqlDbType.Bit
        ).Value = employee.IsActive;

        command.Parameters.Add(
            "@id",
            System.Data.SqlDbType.Int
        ).Value = employee.Id;

        await command.ExecuteNonQueryAsync();
    }
    public async Task AddEmployeeAsync(Employee employee)
    {
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
                @name,
                @department,
                @isActive
            );";

        using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@name",
            System.Data.SqlDbType.NVarChar,
            100
        ).Value = employee.Name;

        command.Parameters.Add(
            "@department",
            System.Data.SqlDbType.NVarChar,
            100
        ).Value = (object?)employee.Department ?? DBNull.Value;

        command.Parameters.Add(
            "@isActive",
            System.Data.SqlDbType.Bit
        ).Value = employee.IsActive;

        await command.ExecuteNonQueryAsync();
    }
}