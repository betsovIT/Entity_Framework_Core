namespace P01_StudentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using Data.Models;
    using static DataSettings;

    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
        }

        public StudentSystemContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                builder.UseSqlServer(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Student>(entity =>
            {
                entity
                    .HasKey(s => s.StudentId);

                entity
                    .Property(s => s.Name)
                    .HasMaxLength(100)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .Property(s => s.PhoneNumber)
                    .HasColumnName("char(10)")
                    .IsRequired(false);

                entity
                    .Property(s => s.RegisteredOn)
                    .IsRequired(true);

                entity
                    .Property(s => s.Birthday)
                    .IsRequired(false);
            });

            builder.Entity<Homework>(entity =>
            {
                entity
                    .HasKey(h => h.HomeworkId);

                entity
                    .Property(h => h.Content)
                    .IsRequired(true)
                    .IsUnicode(false);

                entity
                    .Property(h => h.ContentType)
                    .IsRequired(true);

                entity
                    .Property(h => h.SubmissionTime)
                    .IsRequired(true);

                entity
                    .HasOne(h => h.Student)
                    .WithMany(s => s.HomeworkSubmissions)
                    .HasForeignKey(h => h.StudentId)
                    .IsRequired(true);

                entity
                    .HasOne(h => h.Course)
                    .WithMany(c => c.HomeworkSubmissions)
                    .HasForeignKey(h => h.HomeworkId)
                    .IsRequired(true);
            });

            builder.Entity<Course>(entity =>
            {
                entity
                    .HasKey(c => c.CourseId);

                entity
                    .Property(c => c.Name)
                    .HasMaxLength(80)
                    .IsRequired(true)
                    .IsUnicode(true);

                entity
                    .Property(c => c.Description)
                    .IsUnicode(true)
                    .IsRequired(false);

                entity
                    .Property(c => c.StartDate)
                    .IsRequired(true);

                entity
                    .Property(c => c.EndDate)
                    .IsRequired();

                entity
                    .Property(c => c.Price)
                    .IsRequired(true);
            });

            builder.Entity<StudentCourse>(entity =>
            {
                entity
                    .HasKey(sc => new { sc.CourseId, sc.StudentId });

                entity
                    .HasOne(sc => sc.Course)
                    .WithMany(c => c.StudentsEnrolled)
                    .HasForeignKey(sc => sc.CourseId);

                entity
                    .HasOne(sc => sc.Student)
                    .WithMany(s => s.CourseEnrollments)
                    .HasForeignKey(sc => sc.StudentId);
            });

            builder.Entity<Resource>(entity =>
            {
                entity
                    .HasKey(r => r.ResourceId);

                entity
                    .Property(r => r.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .Property(r => r.Url)
                    .IsRequired(true)
                    .IsUnicode(false);

                entity
                    .Property(r => r.ResourceType)
                    .IsRequired(true);

                entity
                    .HasOne(r => r.Course)
                    .WithMany(c => c.Resources)
                    .HasForeignKey(r => r.CourseId)
                    .IsRequired(true);
            });

        }
    }
}
