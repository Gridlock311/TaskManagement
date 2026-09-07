using System.Data;
using Microsoft.Data.SqlClient;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Services;

public class TaskService
{
    private readonly DatabaseConnection _databaseConnection;

    public TaskService(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }


    // =====================================
    // GET TASK LIST
    // =====================================
    public async Task<List<TaskListItem>> GetTaskListAsync(
        int? employeeId = null,
        int? status = null,
        int? priority = null,
        string? searchText = null)
    {
        List<TaskListItem> tasks = new();

        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        using SqlCommand command =
            new SqlCommand(
                "GetTaskList",
                connection
            );

        command.CommandType =
            CommandType.StoredProcedure;

        command.Parameters.Add(
            "@EmployeeId",
            SqlDbType.Int
        ).Value =
            employeeId.HasValue
                ? employeeId.Value
                : DBNull.Value;

        command.Parameters.Add(
            "@Status",
            SqlDbType.Int
        ).Value =
            status.HasValue
                ? status.Value
                : DBNull.Value;

        command.Parameters.Add(
            "@Priority",
            SqlDbType.Int
        ).Value =
            priority.HasValue
                ? priority.Value
                : DBNull.Value;

        command.Parameters.Add(
            "@SearchText",
            SqlDbType.NVarChar,
            200
        ).Value =
            string.IsNullOrWhiteSpace(searchText)
                ? DBNull.Value
                : searchText;

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            TaskListItem task = new()
            {
                Id = reader.GetInt32(
                    reader.GetOrdinal("Id")
                ),

                Title = reader.GetString(
                    reader.GetOrdinal("Title")
                ),

                EmployeeId = reader.GetInt32(
                    reader.GetOrdinal("EmployeeId")
                ),

                EmployeeName = reader.GetString(
                    reader.GetOrdinal("EmployeeName")
                ),

                Priority = reader.GetInt32(
                    reader.GetOrdinal("Priority")
                ),

                PriorityName = reader.GetString(
                    reader.GetOrdinal("PriorityName")
                ),

                Status = reader.GetInt32(
                    reader.GetOrdinal("Status")
                ),

                StatusName = reader.GetString(
                    reader.GetOrdinal("StatusName")
                ),

                StartDate = reader.GetDateTime(
                    reader.GetOrdinal("StartDate")
                ),

                DueDate = reader.GetDateTime(
                    reader.GetOrdinal("DueDate")
                ),

                CompletedDate =
                    reader.IsDBNull(
                        reader.GetOrdinal("CompletedDate")
                    )
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("CompletedDate")
                    ),

                CreatedDate = reader.GetDateTime(
                    reader.GetOrdinal("CreatedDate")
                ),

                IsOverdue =
                    reader.GetInt32(
                        reader.GetOrdinal("IsOverdue")
                    ) == 1
            };

            tasks.Add(task);
        }

        return tasks;
    }


    // =====================================
    // GET TASK BY ID
    // =====================================
    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
            SELECT
                Id,
                Title,
                Description,
                EmployeeId,
                Priority,
                Status,
                StartDate,
                DueDate,
                CompletedDate,
                CreatedDate
            FROM [Task]
            WHERE Id = @id;";

        using SqlCommand command =
            new SqlCommand(
                sql,
                connection
            );

        command.Parameters.Add(
            "@id",
            SqlDbType.Int
        ).Value = id;

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new TaskItem
        {
            Id = reader.GetInt32(
                reader.GetOrdinal("Id")
            ),

            Title = reader.GetString(
                reader.GetOrdinal("Title")
            ),

            Description =
                reader.IsDBNull(
                    reader.GetOrdinal("Description")
                )
                ? null
                : reader.GetString(
                    reader.GetOrdinal("Description")
                ),

            EmployeeId = reader.GetInt32(
                reader.GetOrdinal("EmployeeId")
            ),

            Priority =
                (TaskPriority)reader.GetInt32(
                    reader.GetOrdinal("Priority")
                ),

            Status =
                (TaskManagement.Models.TaskStatus)
                reader.GetInt32(
                    reader.GetOrdinal("Status")
                ),

            StartDate = reader.GetDateTime(
                reader.GetOrdinal("StartDate")
            ),

            DueDate = reader.GetDateTime(
                reader.GetOrdinal("DueDate")
            ),

            CompletedDate =
                reader.IsDBNull(
                    reader.GetOrdinal("CompletedDate")
                )
                ? null
                : reader.GetDateTime(
                    reader.GetOrdinal("CompletedDate")
                ),

            CreatedDate = reader.GetDateTime(
                reader.GetOrdinal("CreatedDate")
            )
        };
    }


    // =====================================
    // CREATE TASK
    // =====================================
    public async Task CreateTaskAsync(TaskItem task)
    {
        // Rule 1
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            throw new Exception(
                "Title хоосон байж болохгүй."
            );
        }

        // Rule 2
        if (task.DueDate < task.StartDate)
        {
            throw new Exception(
                "DueDate нь StartDate-аас өмнө байж болохгүй."
            );
        }

        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        // Rule 3
        // Employee active эсэхийг шалгана
        string employeeSql = @"
            SELECT
                IsActive
            FROM Employee
            WHERE Id = @employeeId;";

        using SqlCommand employeeCommand =
            new SqlCommand(
                employeeSql,
                connection
            );

        employeeCommand.Parameters.Add(
            "@employeeId",
            SqlDbType.Int
        ).Value = task.EmployeeId;

        object? result =
            await employeeCommand.ExecuteScalarAsync();

        if (result == null)
        {
            throw new Exception(
                "Employee олдсонгүй."
            );
        }

        if (!(bool)result)
        {
            throw new Exception(
                "Inactive Employee дээр Task үүсгэж болохгүй."
            );
        }

        string sql = @"
            INSERT INTO [Task]
            (
                Title,
                Description,
                EmployeeId,
                Priority,
                Status,
                StartDate,
                DueDate,
                CompletedDate,
                CreatedDate
            )
            VALUES
            (
                @title,
                @description,
                @employeeId,
                @priority,
                @status,
                @startDate,
                @dueDate,
                NULL,
                GETDATE()
            );";

        using SqlCommand command =
            new SqlCommand(
                sql,
                connection
            );

        command.Parameters.Add(
            "@title",
            SqlDbType.NVarChar,
            200
        ).Value = task.Title;

        command.Parameters.Add(
            "@description",
            SqlDbType.NVarChar,
            -1
        ).Value =
            string.IsNullOrWhiteSpace(
                task.Description
            )
                ? DBNull.Value
                : task.Description;

        command.Parameters.Add(
            "@employeeId",
            SqlDbType.Int
        ).Value = task.EmployeeId;

        command.Parameters.Add(
            "@priority",
            SqlDbType.Int
        ).Value = (int)task.Priority;

        command.Parameters.Add(
            "@status",
            SqlDbType.Int
        ).Value =
            (int)TaskManagement.Models.TaskStatus.New;

        command.Parameters.Add(
            "@startDate",
            SqlDbType.Date
        ).Value = task.StartDate.Date;

        command.Parameters.Add(
            "@dueDate",
            SqlDbType.Date
        ).Value = task.DueDate.Date;

        await command.ExecuteNonQueryAsync();
    }


    // =====================================
    // UPDATE TASK
    // =====================================
    public async Task UpdateTaskAsync(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            throw new Exception(
                "Title хоосон байж болохгүй."
            );
        }

        if (task.DueDate < task.StartDate)
        {
            throw new Exception(
                "DueDate нь StartDate-аас өмнө байж болохгүй."
            );
        }

        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
            UPDATE [Task]
            SET
                Title = @title,
                Description = @description,
                EmployeeId = @employeeId,
                Priority = @priority,
                StartDate = @startDate,
                DueDate = @dueDate
            WHERE Id = @id;";

        using SqlCommand command =
            new SqlCommand(
                sql,
                connection
            );

        command.Parameters.Add(
            "@title",
            SqlDbType.NVarChar,
            200
        ).Value = task.Title;

        command.Parameters.Add(
            "@description",
            SqlDbType.NVarChar,
            -1
        ).Value =
            string.IsNullOrWhiteSpace(
                task.Description
            )
                ? DBNull.Value
                : task.Description;

        command.Parameters.Add(
            "@employeeId",
            SqlDbType.Int
        ).Value = task.EmployeeId;

        command.Parameters.Add(
            "@priority",
            SqlDbType.Int
        ).Value = (int)task.Priority;

        command.Parameters.Add(
            "@startDate",
            SqlDbType.Date
        ).Value = task.StartDate.Date;

        command.Parameters.Add(
            "@dueDate",
            SqlDbType.Date
        ).Value = task.DueDate.Date;

        command.Parameters.Add(
            "@id",
            SqlDbType.Int
        ).Value = task.Id;

        await command.ExecuteNonQueryAsync();
    }


    // =====================================
    // CHANGE TASK STATUS
    // =====================================
    public async Task ChangeTaskStatusAsync(
        int id,
        TaskManagement.Models.TaskStatus newStatus)
    {
        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        // Одоогийн Status-ийг авна
        string currentSql = @"
            SELECT
                Status
            FROM [Task]
            WHERE Id = @id;";

        using SqlCommand currentCommand =
            new SqlCommand(
                currentSql,
                connection
            );

        currentCommand.Parameters.Add(
            "@id",
            SqlDbType.Int
        ).Value = id;

        object? result =
            await currentCommand.ExecuteScalarAsync();

        if (result == null)
        {
            throw new Exception(
                "Task олдсонгүй."
            );
        }

        var currentStatus =
            (TaskManagement.Models.TaskStatus)
            Convert.ToInt32(result);


        // Rule 5
        // Completed -> New шууд болохгүй
        if (
            currentStatus ==
                TaskManagement.Models.TaskStatus.Completed
            &&
            newStatus ==
                TaskManagement.Models.TaskStatus.New
        )
        {
            throw new Exception(
                "Completed Task-ийг шууд New болгож болохгүй."
            );
        }


        string sql;

        // Rule 4
        // Completed болоход CompletedDate автоматаар бөглөгдөнө
        if (
            newStatus ==
            TaskManagement.Models.TaskStatus.Completed
        )
        {
            sql = @"
                UPDATE [Task]
                SET
                    Status = @status,
                    CompletedDate = GETDATE()
                WHERE Id = @id;";
        }
        else
        {
            sql = @"
                UPDATE [Task]
                SET
                    Status = @status,
                    CompletedDate = NULL
                WHERE Id = @id;";
        }

        using SqlCommand command =
            new SqlCommand(
                sql,
                connection
            );

        command.Parameters.Add(
            "@status",
            SqlDbType.Int
        ).Value = (int)newStatus;

        command.Parameters.Add(
            "@id",
            SqlDbType.Int
        ).Value = id;

        await command.ExecuteNonQueryAsync();
    }


    // =====================================
    // DELETE TASK
    // =====================================
    public async Task DeleteTaskAsync(int id)
    {
        using SqlConnection connection =
            _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        string sql = @"
            DELETE FROM [Task]
            WHERE Id = @id;";

        using SqlCommand command =
            new SqlCommand(
                sql,
                connection
            );

        command.Parameters.Add(
            "@id",
            SqlDbType.Int
        ).Value = id;

        await command.ExecuteNonQueryAsync();
    }
}