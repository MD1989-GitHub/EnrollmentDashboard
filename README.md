# Enrollment Dashboard Application

A modern ASP.NET Core MVC application for managing student enrollments and course registrations.

## Project Structure

```
EnrollmentDashboard/
├── EnrollmentDashboardApplication/          # Main MVC Application
│   ├── Controllers/                         # MVC Controllers ( EnrollmentController)
│   ├── Models/                              # Data Models (Enrollment, Program, Participant)
│   ├── Views/                               # Razor Views ( Index, Details)
│   ├── Services/                            # Business Logic ( EnrollmentService)
│   ├── Program.cs                           # Application Entry Point
│   ├── appsettings.json                     # Configuration
│   └── EnrollmentDashboardApplication.csproj
│
└── EnrollmentDashboardApplication.Tests/    # xUnit Test Project
    ├── EnrollmentServiceTests.cs            # Service Layer Tests
    └── EnrollmentDashboardApplication.Tests.csproj
```

## Technologies Used

- **.NET 8.0** - Latest stable .NET framework
- **ASP.NET Core MVC** - Web application framework
- **Entity Framework Core** - ORM for data access
- **xUnit** - Unit testing framework
- **Moq** - Mocking library for unit tests

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 / VS Code
- SQL Server (LocalDB or Express)

### Building the Project

```bash
dotnet build
```

### Running Unit Tests

```bash
dotnet test
```

Or from the solution directory:

```bash
dotnet test EnrollmentDashboardApplication.Tests/EnrollmentDashboardApplication.Tests.csproj
```

### Running the Application

```bash
cd EnrollmentDashboardApplication
dotnet run
```

The application will be available at `https://localhost:5001` (or the configured port).

## API Features

### **Dashboard** (`/Enrollments`)
   - List of enrollments (participant, program, date, status)
   - Summary statistics (total, active, completed, withdrawn)
   - Filter by date range

### **Detail View** (`/Enrollments/Details/{id}`)
   - Full enrollment information
   - Participant and program details
   - Notes (securely rendered)

## Testing

The project includes comprehensive unit tests using xUnit:

- **EnrollmentServiceTests** - Tests for enrollment business logic
  - Enrollment validation
  - Capacity checking
  - Student and course retrieval

Run tests with:

```bash
dotnet test
```

## Configuration

Configuration is managed through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=EnrollmentDashboardApplication;Trusted_Connection=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## Future Enhancements

- [ ] Database persistence with Entity Framework Core
- [ ] Authentication and authorization
- [ ] Email notifications
- [ ] Student progress tracking
- [ ] Course prerequisites
- [ ] Payment processing integration

## License

This project is provided as-is for educational purposes.


## **Find all security vulnerabilities in this code:**

```csharp
// Controller
public ActionResult SearchEnrollments(string participantName, string startDate, string endDate)
{
    using (var db = new ApplicationDbContext())
    {
        var sql = "SELECT e.*, p.FirstName, p.LastName, pr.ProgramName " +
                  "FROM Enrollments e " +
                  "INNER JOIN Participants p ON e.ParticipantID = p.ParticipantID " +
                  "INNER JOIN Programs pr ON e.ProgramID = pr.ProgramID " +
                  "WHERE 1=1 ";

        if (!string.IsNullOrEmpty(participantName))
        {
            sql += "AND (p.FirstName LIKE '%" + participantName + "%' " +
                   "OR p.LastName LIKE '%" + participantName + "%') ";
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            sql += "AND e.EnrollmentDate >= '" + startDate + "' ";
        }

        if (!string.IsNullOrEmpty(endDate))
        {
            sql += "AND e.EnrollmentDate <= '" + endDate + "' ";
        }

        var results = db.Database.SqlQuery<EnrollmentViewModel>(sql).ToList();
        return View(results);
    }
}

// View (Razor)
@foreach (var enrollment in Model)
{
    <tr>
        <td>@enrollment.FirstName @enrollment.LastName</td>
        <td>@enrollment.ProgramName</td>
        <td>@enrollment.EnrollmentDate.ToShortDateString()</td>
        <td>@Html.Raw(enrollment.Notes)</td>
        <td>
            <a href="/Enrollments/Details?id=@enrollment.EnrollmentID">View</a>
            <a href="/Enrollments/Delete?id=@enrollment.EnrollmentID">Delete</a>
        </td>
    </tr>
}

// Detail view
public ActionResult Details(int id)
{
    using (var db = new ApplicationDbContext())
    {
        var enrollment = db.Enrollments
            .Include(e => e.Participant)
            .Include(e => e.Program)
            .FirstOrDefault(e => e.EnrollmentID == id);

        return View(enrollment);
    }
}

## Review Comments
// Review Comment 1: The code uses string concatenation (+) to build the SQL query using raw user inputs (participantName, startDate, endDate). An attacker can manipulate these parameters to bypass authentication, extract sensitive database schema information, modify data, or execute remote commands ( Critical)

// Review Comment 2 : The Entity Framework method db.Database.SqlQuery<T>() accepts raw string commands. Passing an unvalidated concatenated string directly into this method execution introduces classic SQL injection risks ( High)

//Fix : for 1 and 2 user SQLParameters for adding records

// Review Comment 3: The controller accepts startDate and endDate as plain strings instead of strongly-typed DateTime objects. This allows attackers to inject malicious code payload strings instead of calendar dates (High)

// Fix : Use Strongly typed input parameters

// Review Comment 4: By using @Html.Raw(), you explicitly tell the Razor engine to disable its automatic built-in HTML encoding protection. If an attacker inputs a malicious JavaScript payload into the Notes field (e.g., during creation or edit), that script will execute directly in the browser of anyone viewing this dashboard. 
// Fix : <td>@enrollment.Notes</td>

// Review Comment 5 : The Details and Delete actions trust the incoming raw integer id string directly from the URL route query parameters (/Enrollments/Details?id=123). There are no security boundary validations confirming if the currently logged-in user actually owns or has permission to view that specific record.
// Fix : use [Authorize]


```
