namespace TeisterMask.Data
{
    using Microsoft.EntityFrameworkCore;
    using TeisterMask.Data.Models;

    public class TeisterMaskContext : DbContext
    {
        public TeisterMaskContext() { }

        public TeisterMaskContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<EmployeeTask> EmployeesTasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>(task =>
            {
                task.HasOne(t => t.Project).WithMany(p => p.Tasks).HasForeignKey(t => t.ProjectId);
            });

            modelBuilder.Entity<EmployeeTask>(empTask =>
            {
                empTask.HasKey(et => new { et.EmployeeId, et.TaskId });

                empTask.HasOne(et => et.Employee).WithMany(e => e.EmployeesTasks).HasForeignKey(et => et.EmployeeId);
                empTask.HasOne(et => et.Task).WithMany(t => t.EmployeesTasks).HasForeignKey(et => et.TaskId);
            });
        }
    }
}