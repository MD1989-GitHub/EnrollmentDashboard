using EnrollmentDashboardApplication.Models;
using EnrollmentDashboardApplication.Data;

namespace EnrollmentDashboardApplication.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository  _enrollmentRepo = null!;
 
    public EnrollmentService(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepo = enrollmentRepository;        
    }
    

    public Task<List<Enrollment>> GetEnrollmentDetailsAsync(SearchFilterViewModel filterViewModel)
    {
        return Task.FromResult(_enrollmentRepo.GetFilteredEnrollmentDetailsByAsync(filterViewModel).Result);
    }
    
    public Task<Enrollment?> GetEnrollmentDetailsByIdAsync(int id)
    {
        return Task.FromResult(_enrollmentRepo.GetEnrollmentDetailsByIdAsync(id).Result);
    }    
}