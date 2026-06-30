using System.ComponentModel.DataAnnotations;
namespace EnrollmentDashboardApplication.Models;
public class Participant
{
    public int ParticipantId { get; set; }

    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";

    public DateTime DateOfBirth { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();

    public string FullName => $"{FirstName} {LastName}";
}
