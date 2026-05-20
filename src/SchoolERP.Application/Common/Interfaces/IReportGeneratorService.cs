namespace SchoolERP.Application.Common.Interfaces;

// ── Service interface ────────────────────────────────────────────────────────

public interface IReportGeneratorService
{
    byte[] GenerateAttendanceReport(AttendanceReportData data);
    byte[] GenerateReportCard(ReportCardData data);
    byte[] GenerateFinancialReport(FinancialReportData data);
    byte[] GenerateEnrollmentReport(EnrollmentReportData data);
}

// ── Data models (POCO bags passed from Application → Infrastructure) ─────────

public record AttendanceReportData(
    string SchoolName,
    string SectionName,
    string GradeLevel,
    string PeriodName,
    DateTime StartDate,
    DateTime EndDate,
    DateTime GeneratedAt,
    IReadOnlyList<StudentAttendanceRow> Rows);

public record StudentAttendanceRow(
    int Index,
    string FullName,
    string? StudentCode,
    int Present,
    int Absent,
    int Late,
    int Excused,
    int TotalDays,
    decimal AttendanceRate);

public record ReportCardData(
    string SchoolName,
    string StudentFullName,
    string? StudentCode,
    string GradeLevel,
    string SectionName,
    string PeriodName,
    string AcademicYear,
    DateTime GeneratedAt,
    IReadOnlyList<SubjectGradeRow> Subjects,
    decimal GeneralAverage,
    bool IsPromoted,
    int? Rank,
    int TotalStudents,
    AttendanceSummaryData? Attendance);

public record SubjectGradeRow(
    string SubjectName,
    IReadOnlyList<EvalItemRow> Evaluations,
    decimal WeightedAverage,
    string LetterGrade,
    bool IsPassing);

public record EvalItemRow(
    string Name,
    decimal Weight,
    decimal Score);

public record AttendanceSummaryData(
    int Present,
    int Absent,
    int Late,
    int Excused,
    decimal AttendanceRate);

public record FinancialReportData(
    string SchoolName,
    DateTime DateFrom,
    DateTime DateTo,
    DateTime GeneratedAt,
    decimal TotalBilled,
    decimal TotalCollected,
    decimal TotalPending,
    decimal TotalOverdue,
    decimal CollectionRate,
    IReadOnlyList<MonthlyFinancialRow> Monthly,
    IReadOnlyList<PaymentMethodRow> PaymentMethods);

public record MonthlyFinancialRow(
    string Month,
    decimal Billed,
    decimal Collected,
    decimal Pending);

public record PaymentMethodRow(
    string Method,
    decimal Amount,
    decimal Percentage);

public record EnrollmentReportData(
    string SchoolName,
    string AcademicYearName,
    DateTime GeneratedAt,
    int TotalEnrolled,
    IReadOnlyList<GradeLevelEnrollmentRow> GradeLevels);

public record GradeLevelEnrollmentRow(
    string GradeName,
    IReadOnlyList<SectionEnrollmentRow> Sections,
    int TotalEnrolled,
    int TotalMale,
    int TotalFemale);

public record SectionEnrollmentRow(
    string SectionName,
    int Enrolled,
    int Male,
    int Female);
