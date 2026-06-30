using System.ComponentModel.DataAnnotations;

namespace EnrollmentDashboardApplication.Models;

public class SearchFilterViewModel : IValidatableObject
{
    // Filter properties to retain user selections in the form inputs
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }
    public EnrollmentStatus? Status { get; set; }

    // Summary Statistics
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int CompletedCount { get; set; }
    public int WithdrawnCount { get; set; }

    // The core filtered data list
    public IEnumerable<Enrollment>? Enrollments { get; set; } = null!;

    // 2. The cross-field validation method execution
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
        {
            // You can optionally pass the field name so the error highlights the exact input box
            yield return new ValidationResult(
                "Start Date cannot be greater than End Date.", 
                new[] { nameof(StartDate) }
            );
        }
    }
}