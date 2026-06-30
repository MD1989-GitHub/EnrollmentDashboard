using EnrollmentDashboardApplication.Models;

namespace EnrollmentDashboardApplication.Data;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetEnrollmentDetailsByIdAsync(int id);

    Task<List<Enrollment>> GetFilteredEnrollmentDetailsByAsync(SearchFilterViewModel filter);
}
