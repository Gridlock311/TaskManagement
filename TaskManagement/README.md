# Task Management System

## Project Description
Employee-д task үүсгэж, хариуцагч оноож, task-ийн status, priority, хугацааг хянах жижиг business application.

## Technologies
- C#
- .NET
- Blazor
- SQL Server
- Git

## Architecture
- Models
- Services
- Data
- Components / Pages
- Sql

## Database Structure
### Employee
- Id
- Name
- Department
- IsActive

### Task
- Id
- Title
- Description
- EmployeeId
- Priority
- Status
- StartDate
- DueDate
- CompletedDate
- CreatedDate

## Main Features
- Employee list
- Add employee
- Edit employee
- Deactivate employee
- Search employee
- Task list
- Create task
- Edit task
- Delete task
- Change task status
- Task filters
- Dashboard
- Overdue task detection

## Business Rules
1. Title хоосон байж болохгүй.
2. DueDate нь StartDate-аас өмнө байж болохгүй.
3. Inactive Employee дээр шинэ Task үүсгэж болохгүй.
4. Completed болох үед CompletedDate автоматаар бөглөгдөнө.
5. Completed Task-ийг шууд New болгож болохгүй.

## How to Run
1. SQL Server дээр `CreateDatabase.sql` ажиллуулна.
2. `StoredProcedures.sql` ажиллуулна.
3. `appsettings.json` дээр connection string тохируулна.
4. Visual Studio дээр project нээнэ.
5. Run хийнэ.

## AI Usage
AI assistant-ийг кодын алдааг хянахад ашиглав. Мөн зарим кодын алдааг олохгүй тохиолдолд ашиглав.

## Problems Faced
- SQL Server connection string тохиргоо
- Blazor interactive mode
- Git setup
- Task edit/status logic

## Solutions
- LocalDB server name зөв тохируулсан.
- `InteractiveServer` render mode ашигласан.
- Git repository үүсгэж GitHub руу push хийсэн.
- Business logic-ийг Service layer дотор байрлуулсан.