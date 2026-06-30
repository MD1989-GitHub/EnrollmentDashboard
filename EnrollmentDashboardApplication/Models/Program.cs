using System.ComponentModel.DataAnnotations;
namespace EnrollmentDashboardApplication.Models;

public class Program
{
    public int ProgramId { get; set; }

    [Required]
    [StringLength(100)]
    public string ProgramName { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();
}