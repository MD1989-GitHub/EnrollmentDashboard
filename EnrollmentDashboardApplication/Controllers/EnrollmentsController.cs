using EnrollmentDashboardApplication.Models;
using EnrollmentDashboardApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentDashboardApplication.Controllers;

public class EnrollmentsController : Controller
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(SearchFilterViewModel filter)
    {
        // 1. Run your validation block rules
        if (!ModelState.IsValid)
        {
            // If validation fails (e.g. Start Date > End Date), return the model immediately to show errors
            filter.Enrollments = new List<Enrollment>();
            return View(filter);
        }

        var matchingEnrollments = await _enrollmentService.GetEnrollmentDetailsAsync(filter);
        // 3. Bind the matching records directly to your view model property
    filter.Enrollments = matchingEnrollments;

    // 4. Calculate your dashboard totals dynamically from the list results
    filter.TotalCount = matchingEnrollments.Count;
    filter.ActiveCount = matchingEnrollments.Count(e => e.Status == EnrollmentStatus.Active);
    filter.CompletedCount = matchingEnrollments.Count(e => e.Status == EnrollmentStatus.Completed);
    filter.WithdrawnCount = matchingEnrollments.Count(e => e.Status == EnrollmentStatus.Withdrawn);

    // 5. FIX: Pass the fully populated view model into the view, NOT just the list!
    return View(filter);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var enrollment = await _enrollmentService.GetEnrollmentDetailsByIdAsync(id);
        if (enrollment == null)
        {
            return NotFound();
        }         

        return View(enrollment);
    }
}
