CREATE OR ALTER PROCEDURE dbo.usp_GetEnrollments
(
      @StartDate  DATE = NULL
    , @EndDate    DATE = NULL
    , @Status     INT = 0
    , @PageNumber INT = 1
    , @PageSize   INT = 50
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        -------------------------------------------------------
        -- Input Validation
        -------------------------------------------------------
        IF (@StartDate IS NOT NULL
            AND @EndDate IS NOT NULL
            AND @StartDate > @EndDate)
        BEGIN
            THROW 50001, 'Start Date cannot be greater than End Date.', 1;
        END;

        -------------------------------------------------------
        -- Validate Status
        -------------------------------------------------------
        IF (@Status IS NOT NULL
            AND @Status NOT IN (0, 1, 2))
        BEGIN
            THROW 50002, 'Invalid Status supplied.', 1;
        END;

        -------------------------------------------------------
        -- Pagination Parameters Validation & Setup
        -------------------------------------------------------
        -- Enforce sensible performance minimums for paging
        IF (@PageNumber < 1) SET @PageNumber = 1;
        IF (@PageSize < 1) SET @PageSize = 50;

        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

        -------------------------------------------------------
        -- Paginated Dashboard Query
        -------------------------------------------------------
        SELECT
            e.EnrollmentId,
            p.ParticipantId,
            CONCAT(p.FirstName, ' ', p.LastName) AS ParticipantName,
            pr.ProgramId,
            pr.ProgramName,
            e.EnrollmentDate,
            e.Status,
            e.Notes,
            COUNT(*) OVER() AS TotalRecords -- Returns the overall count for UI pagination
        FROM dbo.Enrollments e
        INNER JOIN dbo.Participants p ON e.ParticipantId = p.ParticipantId
        INNER JOIN dbo.Programs pr ON e.ProgramId = pr.ProgramId
        WHERE
            (@StartDate IS NULL OR @StartDate = '0001-01-01' OR e.EnrollmentDate >= @StartDate)
        AND
            (@EndDate IS NULL OR @EndDate = '0001-01-01' OR e.EnrollmentDate <= @EndDate)
        AND
            (@Status IS NULL OR e.Status = @Status)
        ORDER BY
            e.EnrollmentDate DESC,
            p.LastName,
            p.FirstName
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    END TRY
    BEGIN CATCH
        -- Cleanly bubbles up any database, connection, or custom errors
        THROW; 
    END CATCH
END
GO
