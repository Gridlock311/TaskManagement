USE TaskManagementDb;
GO

-- 1. Employee бүрийн нийт Task
SELECT
    B.Id,
    B.Name,
    COUNT(A.Id) AS TotalTaskCount
FROM Employee B
LEFT OUTER JOIN [Task] A
    ON B.Id = A.EmployeeId
GROUP BY
    B.Id,
    B.Name;
GO


-- 2. Employee бүрийн Completed Task
SELECT
    B.Id,
    B.Name,
    COUNT(A.Id) AS CompletedTaskCount
FROM Employee B
LEFT OUTER JOIN [Task] A
    ON B.Id = A.EmployeeId
    AND A.Status = 3
GROUP BY
    B.Id,
    B.Name;
GO


-- 3. Overdue Task
SELECT
    Id,
    Title,
    EmployeeName,
    PriorityName,
    StatusName,
    StartDate,
    DueDate
FROM TaskView
WHERE IsOverdue = 1;
GO


-- 4. TOP 5 хамгийн олон Task-тай Employee
SELECT TOP 5
    B.Id,
    B.Name,
    COUNT(A.Id) AS TotalTaskCount
FROM Employee B
LEFT OUTER JOIN [Task] A
    ON B.Id = A.EmployeeId
GROUP BY
    B.Id,
    B.Name
ORDER BY
    TotalTaskCount DESC;
GO


-- 5. Status бүрийн Task count
SELECT
    Status,
    CASE Status
        WHEN 1 THEN N'New'
        WHEN 2 THEN N'In Progress'
        WHEN 3 THEN N'Completed'
        WHEN 4 THEN N'Cancelled'
    END AS StatusName,
    COUNT(Id) AS TaskCount
FROM [Task]
GROUP BY
    Status;
GO