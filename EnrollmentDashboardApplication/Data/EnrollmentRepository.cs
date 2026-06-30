using EnrollmentDashboardApplication.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EnrollmentDashboardApplication.Data;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    // Get Enrollment Details by Id
    public async Task<Enrollment?> GetEnrollmentDetailsByIdAsync(int id)
    {
        return await _context.Enrollments
            .Include(x => x.Participant)
            .Include(x => x.Program)            
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EnrollmentId == id);
    }

    // Get All Lists as per StartDate, EndDate, Status    
public async Task<List<Enrollment>> GetFilteredEnrollmentDetailsByAsync(SearchFilterViewModel filter)
{
    var list = new List<Enrollment>();

    // 1. Create a clean command bound to your DbContext connection
    using (var command = _context.Database.GetDbConnection().CreateCommand())
    {
        command.CommandText = "dbo.usp_GetEnrollments";
        command.CommandType = CommandType.StoredProcedure;

        // 2. Attach incoming viewmodel filters smoothly
        command.Parameters.Add(new SqlParameter("@StartDate", filter.StartDate.HasValue ? (object)filter.StartDate.Value : DBNull.Value));
        command.Parameters.Add(new SqlParameter("@EndDate", filter.EndDate.HasValue ? (object)filter.EndDate.Value : DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Status", filter.Status ?? EnrollmentStatus.Active));

        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync();

        // 3. Read the columns and assemble the objects safely
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var enrollment = new Enrollment
                {
                    EnrollmentId = reader.GetInt32("EnrollmentId"),
                    EnrollmentDate = reader.GetDateTime("EnrollmentDate"),
                    Status = Enum.Parse<EnrollmentStatus>(reader.GetString("Status")),
                    
                    // Wire up the child property so the view can read it
                    Participant = new Participant
                    {
                        ParticipantId = reader.GetInt32("ParticipantId"),
                        FirstName = reader.GetString("ParticipantName"), // Maps flat full name
                        Email = reader.GetString("Email")
                    },
                    Program = new Models.Program
                    {
                        ProgramId = reader.GetInt32("ProgramID"), 
                        ProgramName = reader.GetString("ProgramName"),
                        Description = reader.GetString("Description")
                    }
                };
                list.Add(enrollment);
            }
        }
    }
    return list;
}
}