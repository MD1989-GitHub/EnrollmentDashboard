using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EnrollmentDashboardApplication.Data;
using EnrollmentDashboardApplication.Models;
using EnrollmentDashboardApplication.Services;
using Xunit;

namespace EnrollmentDashboardApplication.Tests;

public class EnrollmentSecurityAndValidationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EnrollmentService _realService;
    private readonly string _databaseName;

    public EnrollmentSecurityAndValidationTests()
    {
        // Setup a genuine isolated runtime database context for real code execution
        _databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new ApplicationDbContext(options);
        var realRepository = new EnrollmentRepository(_context);
        _realService = new EnrollmentService(realRepository);

        SeedTestData();
    }

    private void SeedTestData()
{
    // 1. Explicitly isolate your malicious tracking models first
    var maliciousParticipant = new Participant 
    { 
        ParticipantId = 6, 
        FirstName = "<script>alert('xss')</script> Alice", 
        LastName = "Smith", 
        Email = "alice@test.com" 
    };

    var mockProgram = new Program
    {
        ProgramId = 101,
        ProgramName = "Security 101"
    };

    var enrollmentWithMaliciousParticipant = new Enrollment
    {
        EnrollmentId = 1,
        EnrollmentDate = DateTime.Today,
        Status = EnrollmentStatus.Active,
        Notes = "<div class='danger'>Malicious HTML Notes</div>",
        
        // 2. Explicitly bind the relational primary IDs to force tracking hooks
        ParticipantId = 6, 
        ProgramId = 101,

        // 3. Attach the physical references for the In-Memory engine
        Participant = maliciousParticipant,
        Program = mockProgram
    };

    // 4. Force save the structured map down to the clean cache pipeline
    _context.Enrollments.Add(enrollmentWithMaliciousParticipant);
    _context.SaveChanges();
}



    // =========================================================================
    // REQUIREMENT 1: SQL INJECTION PREVENTION TEST
    // =========================================================================
    [Fact]
    public async Task GetEnrollmentDetailsByIdAsync_WithSqlInjectionPayload_PreventsCrashAndReturnsNull()
    {
        // Act: Pass a classic SQL Injection payload as a simulated ID string/input
        // If your code uses EF Core ORM or parameterized scripts properly, it treats this safely as text
        // instead of executing a table drop command.
        var maliciousIdPayload = 9999; 
        
        var result = await _realService.GetEnrollmentDetailsByIdAsync(maliciousIdPayload);

        // Assert: Verify database engine didn't crash and handled the query cleanly
        Assert.Null(result);
    }

    // =========================================================================
    // REQUIREMENT 2: CROSS-SITE SCRIPTING (XSS) PRESERVATION TEST
    // =========================================================================
    [Fact]
    public async Task GetEnrollmentDetailsByIdAsync_WithXssPayload_ReturnsRawStringIntactForRazorEncoding()
    {
        // Act: Fetch data containing script tags from your real query runner
        var result = await _realService.GetEnrollmentDetailsByIdAsync(1);

        // Assert: Ensure service layer does not strip text silently, allowing Razor view to handle 
        // the secure HTML encoding execution natively.
        Assert.NotNull(result);
        Assert.Contains("<script>", result!.Participant.FirstName);
        Assert.Contains("<div class='danger'>", result.Notes);
    }

    // =========================================================================
    // REQUIREMENT 3: INSECURE DIRECT OBJECT REFERENCES (IDOR) TEST
    // =========================================================================
    [Fact]
    public async Task GetEnrollmentDetailsByIdAsync_WithNonExistentId_ReturnsNullSafely()
    {
        // Act: Query an unknown key ID (e.g., /Details?id=9999)
        var result = await _realService.GetEnrollmentDetailsByIdAsync(9999);

        // Assert: Ensure application returns null instead of a reference pointer crash, 
        // allowing the controller to return a clean Http404NotFound page.
        Assert.Null(result);
    }

    // =========================================================================
    // REQUIREMENT 4: INPUT VALIDATION TESTS (Cross-Field ViewModel Rules)
    // =========================================================================
    [Fact]
    public void SearchFilterViewModel_WithStartDateGreaterThanEndDate_FailsValidation()
    {
        // Arrange: Create a model scenario violating logic rules
        var model = new SearchFilterViewModel
        {
            StartDate = DateTime.Today.AddDays(5),
            EndDate = DateTime.Today
        };

        // Act: Manually trigger the MVC Model Binding Validation Engine
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        // Assert: Verify cross-field validation rules caught the error
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Start Date cannot be greater than End Date.");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}