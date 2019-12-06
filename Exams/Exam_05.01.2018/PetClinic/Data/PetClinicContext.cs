namespace PetClinic.Data
{
    using Microsoft.EntityFrameworkCore;
    using PetClinic.Models;

    public class PetClinicContext : DbContext
    {
        public PetClinicContext() { }

        public PetClinicContext(DbContextOptions options)
            :base(options) { }

        public DbSet<Animal> Animals { get; set; }

        public DbSet<Passport> Passports { get; set; }

        public DbSet<Procedure> Procedures { get; set; }

        public DbSet<Vet> Vets { get; set; }

        public DbSet<ProcedureAnimalAid> ProceduresAnimalAids { get; set; }

        public DbSet<AnimalAid> AnimalAids { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProcedureAnimalAid>(proc =>
            {
                proc.HasKey(paa => new { paa.AnimalAidId, paa.ProcedureId });

                proc.HasOne(paa => paa.AnimalAid).WithMany(aa => aa.AnimalAidProcedures).HasForeignKey(paa => paa.AnimalAidId);
                proc.HasOne(paa => paa.Procedure).WithMany(p => p.ProcedureAnimalAids).HasForeignKey(paa => paa.ProcedureId);
            });

            builder.Entity<AnimalAid>(animalAid =>
            {
                animalAid.HasKey(aa => aa.Id);

                animalAid.HasAlternateKey(aa => aa.Name);
            });

            builder.Entity<Vet>(vet =>
            {
                vet.HasKey(v => v.Id);

                vet.HasAlternateKey(v => v.PhoneNumber);
            });

            builder.Entity<Procedure>(proc =>
            {
                proc.HasOne(p => p.Animal).WithMany(a => a.Procedures).HasForeignKey(p => p.AnimalId);
                proc.HasOne(p => p.Vet).WithMany(v => v.Procedures).HasForeignKey(p => p.VetId);
            });

            builder.Entity<Animal>(animal =>
            {
                animal.HasOne(a => a.Passport).WithOne(p => p.Animal).HasForeignKey<Animal>(a => a.PassportSerialNumber);
            });

            builder.Entity<Passport>(passport =>
            {
                passport.HasKey(p => p.SerialNumber);
            });
        }
    }
}
