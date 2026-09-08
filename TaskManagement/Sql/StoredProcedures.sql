USE TaskManagementDb;
GO

CREATE PROC GetTaskList
(
    @EmployeeId INT = NULL,
    @Status INT = NULL,
    @Priority INT = NULL,
    @SearchText NVARCHAR(200) = NULL
)
AS
BEGIN
    SELECT
        Id,
        Title,
        EmployeeId,
        EmployeeName,
        Priority,
        PriorityName,
        Status,
        StatusName,
        StartDate,
        DueDate,
        CompletedDate,
        CreatedDate,
        IsOverdue
    FROM TaskView
    WHERE
        (@EmployeeId IS NULL OR EmployeeId = @EmployeeId)
        AND (@Status IS NULL OR Status = @Status)
        AND (@Priority IS NULL OR Priority = @Priority)
        AND
        (
            @SearchText IS NULL
            OR Title LIKE N'%' + @SearchText + N'%'
        )
    ORDER BY Id ASC;
END;
GO