using Microsoft.Data.SqlClient;

namespace TaskManagement.Data;

public class DatabaseConnection
{
    private readonly IConfiguration _configuration;

    public DatabaseConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection CreateConnection()
    {
        string connectionString =
            _configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string олдсонгүй.");

        return new SqlConnection(connectionString);
    }
}