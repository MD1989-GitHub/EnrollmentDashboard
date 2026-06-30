using EnrollmentDashboardApplication.Models;

namespace EnrollmentDashboardApplication.Services;

public interface IEnrollmentService
{
    Task<Enrollment?> GetEnrollmentDetailsByIdAsync(int enrollmentId);
    Task<List<Enrollment>> GetEnrollmentDetailsAsync(SearchFilterViewModel filterViewModel);    
}