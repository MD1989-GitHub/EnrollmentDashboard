using System.ComponentModel.DataAnnotations.Schema;
namespace EnrollmentDashboardApplication.Models;

public class Enrollment
{
    public int EnrollmentId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    public string? Notes { get; set; }
    public int ParticipantId { get; set; }
    public int ProgramId { get; set; }

    [ForeignKey("ParticipantID")]
        public Participant Participant { get; set; } = null!;

    [ForeignKey("ProgramID")]
        public Program Program { get; set; } = null!;
}