using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nexora.Authorization.Roles;
using Nexora.Authorization.Users;
using Nexora.Domain.Admissions;
using Nexora.MultiTenancy;

namespace Nexora.EntityFrameworkCore;

public class NexoraDbContext : AbpZeroDbContext<Tenant, Role, User, NexoraDbContext>
{
    // Admissions module
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<Campus> Campuses { get; set; }
    public DbSet<GradeLevel> GradeLevels { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<AdmissionApplication> AdmissionApplications { get; set; }
    public DbSet<Guardian> Guardians { get; set; }
    public DbSet<ApplicationGuardian> ApplicationGuardians { get; set; }
    public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
    public DbSet<AdmissionAssessment> AdmissionAssessments { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentGuardian> StudentGuardians { get; set; }
    public DbSet<EnrollmentHistory> EnrollmentHistories { get; set; }

    public NexoraDbContext(DbContextOptions<NexoraDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AcademicYear>(b =>
        {
            b.Property(e => e.Name).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Campus>(b =>
        {
            b.Property(e => e.Name).HasMaxLength(150).IsRequired();
            b.Property(e => e.City).HasMaxLength(100);
            b.Property(e => e.Country).HasMaxLength(100);
            b.Property(e => e.Address).HasMaxLength(300);
            b.Property(e => e.ContactNumber).HasMaxLength(30);
        });

        modelBuilder.Entity<GradeLevel>(b =>
        {
            b.Property(e => e.Name).HasMaxLength(50).IsRequired();
            b.Property(e => e.Code).HasMaxLength(20);
            b.HasOne(e => e.Campus)
             .WithMany(c => c.GradeLevels)
             .HasForeignKey(e => e.CampusId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Section>(b =>
        {
            b.Property(e => e.Name).HasMaxLength(20).IsRequired();
            b.HasOne(e => e.GradeLevel)
             .WithMany(g => g.Sections)
             .HasForeignKey(e => e.GradeLevelId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AdmissionApplication>(b =>
        {
            b.Property(e => e.ApplicationNumber).HasMaxLength(30).IsRequired();
            b.HasIndex(e => new { e.TenantId, e.ApplicationNumber }).IsUnique();
            b.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            b.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            b.Property(e => e.Nationality).HasMaxLength(100);
            b.Property(e => e.Religion).HasMaxLength(50);
            b.Property(e => e.PreviousSchoolName).HasMaxLength(200);
            b.Property(e => e.PreviousSchoolBoard).HasMaxLength(100);
            b.Property(e => e.PreviousGradeLevel).HasMaxLength(50);
            b.Property(e => e.ReviewNotes).HasMaxLength(1000);
            b.Property(e => e.RejectionReason).HasMaxLength(500);
            b.HasOne(e => e.AcademicYear).WithMany().HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.Campus).WithMany().HasForeignKey(e => e.CampusId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.DesiredGradeLevel).WithMany().HasForeignKey(e => e.DesiredGradeLevelId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Guardian>(b =>
        {
            b.Property(e => e.FullName).HasMaxLength(150).IsRequired();
            b.Property(e => e.NationalIdNumber).HasMaxLength(50);
            b.Property(e => e.Email).HasMaxLength(150);
            b.Property(e => e.Phone).HasMaxLength(30);
            b.Property(e => e.Occupation).HasMaxLength(100);
            b.Property(e => e.Address).HasMaxLength(300);
        });

        modelBuilder.Entity<ApplicationGuardian>(b =>
        {
            b.HasIndex(e => new { e.ApplicationId, e.GuardianId }).IsUnique();
            b.HasOne(e => e.Application)
             .WithMany(a => a.ApplicationGuardians)
             .HasForeignKey(e => e.ApplicationId)
             .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.Guardian)
             .WithMany()
             .HasForeignKey(e => e.GuardianId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApplicationDocument>(b =>
        {
            b.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            b.Property(e => e.FileUrl).HasMaxLength(500);
            b.Property(e => e.RejectionNote).HasMaxLength(300);
            b.HasOne(e => e.Application)
             .WithMany(a => a.Documents)
             .HasForeignKey(e => e.ApplicationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AdmissionAssessment>(b =>
        {
            b.Property(e => e.Remarks).HasMaxLength(1000);
            b.Property(e => e.Score).HasColumnType("decimal(10,2)");
            b.Property(e => e.MaxScore).HasColumnType("decimal(10,2)");
            b.HasOne(e => e.Application)
             .WithMany(a => a.Assessments)
             .HasForeignKey(e => e.ApplicationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Student>(b =>
        {
            b.Property(e => e.RegistrationNumber).HasMaxLength(30).IsRequired();
            b.HasIndex(e => new { e.TenantId, e.RegistrationNumber }).IsUnique();
            b.Property(e => e.GRNumber).HasMaxLength(30);
            b.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            b.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            b.Property(e => e.BloodGroup).HasMaxLength(5);
            b.Property(e => e.Nationality).HasMaxLength(100);
            b.Property(e => e.Religion).HasMaxLength(50);
            b.Property(e => e.PhotoUrl).HasMaxLength(500);
            b.HasOne(e => e.Application).WithMany().HasForeignKey(e => e.ApplicationId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(e => e.Campus).WithMany().HasForeignKey(e => e.CampusId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.CurrentGradeLevel).WithMany().HasForeignKey(e => e.CurrentGradeLevelId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.CurrentSection).WithMany().HasForeignKey(e => e.CurrentSectionId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<StudentGuardian>(b =>
        {
            b.HasIndex(e => new { e.StudentId, e.GuardianId }).IsUnique();
            b.HasOne(e => e.Student)
             .WithMany(s => s.StudentGuardians)
             .HasForeignKey(e => e.StudentId)
             .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.Guardian)
             .WithMany()
             .HasForeignKey(e => e.GuardianId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EnrollmentHistory>(b =>
        {
            b.Property(e => e.Remarks).HasMaxLength(300);
            b.HasOne(e => e.Student)
             .WithMany(s => s.EnrollmentHistories)
             .HasForeignKey(e => e.StudentId)
             .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.AcademicYear).WithMany().HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.GradeLevel).WithMany().HasForeignKey(e => e.GradeLevelId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.Section).WithMany().HasForeignKey(e => e.SectionId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });
    }
}
