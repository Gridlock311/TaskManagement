CREATE DATABASE TaskManagementDb;
GO

USE TaskManagementDb;
GO


-- =====================================
-- EMPLOYEE TABLE
-- =====================================

CREATE TABLE Employee
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100) NULL,
    IsActive BIT NOT NULL,

    CONSTRAINT PK_Employee
        PRIMARY KEY CLUSTERED (Id ASC)
);
GO


-- =====================================
-- TASK TABLE
-- =====================================

CREATE TABLE [Task]
(
    Id INT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    EmployeeId INT NOT NULL,
    Priority INT NOT NULL,
    Status INT NOT NULL,
    StartDate DATE NOT NULL,
    DueDate DATE NOT NULL,
    CompletedDate DATETIME NULL,
    CreatedDate DATETIME NOT NULL,

    CONSTRAINT PK_Task
        PRIMARY KEY CLUSTERED (Id ASC)
);
GO


-- =====================================
-- FOREIGN KEY
-- =====================================

ALTER TABLE [Task]
WITH CHECK
ADD CONSTRAINT FK_Task_Employee
FOREIGN KEY (EmployeeId)
REFERENCES Employee (Id);
GO


-- =====================================
-- INDEX
-- =====================================

CREATE NONCLUSTERED INDEX IDX_Task_EmployeeId
ON [Task]
(
    EmployeeId ASC
);
GO


-- =====================================
-- TASK VIEW
-- =====================================

CREATE VIEW TaskView
AS
SELECT
    A.Id,
    A.Title,
    A.Description,
    A.EmployeeId,
    B.Name AS EmployeeName,

    A.Priority,

    CASE A.Priority
        WHEN 1 THEN N'Low'
        WHEN 2 THEN N'Normal'
        WHEN 3 THEN N'High'
        WHEN 4 THEN N'Critical'
    END AS PriorityName,

    A.Status,

    CASE A.Status
        WHEN 1 THEN N'New'
        WHEN 2 THEN N'In Progress'
        WHEN 3 THEN N'Completed'
        WHEN 4 THEN N'Cancelled'
    END AS StatusName,

    A.StartDate,
    A.DueDate,
    A.CompletedDate,
    A.CreatedDate,

    CASE
        WHEN A.DueDate < CAST(GETDATE() AS DATE)
             AND A.Status <> 3
        THEN 1
        ELSE 0
    END AS IsOverdue

FROM [Task] A
INNER JOIN Employee B
    ON A.EmployeeId = B.Id;
GO