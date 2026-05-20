using Microsoft.EntityFrameworkCore;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<School> Schools { get; }
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Guardian> Guardians { get; }
    DbSet<Student> Students { get; }
    DbSet<StudentGuardian> StudentGuardians { get; }
    DbSet<Teacher> Teachers { get; }
    DbSet<TeacherAssignment> TeacherAssignments { get; }
    DbSet<AcademicYear> AcademicYears { get; }
    DbSet<AcademicPeriod> AcademicPeriods { get; }
    DbSet<GradeLevel> GradeLevels { get; }
    DbSet<Section> Sections { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<AttendanceRecord> AttendanceRecords { get; }
    DbSet<EvaluationPlan> EvaluationPlans { get; }
    DbSet<GradeEntry> GradeEntries { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Announcement> Announcements { get; }
    DbSet<Message> Messages { get; }
    DbSet<ScheduleSlot> ScheduleSlots { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
