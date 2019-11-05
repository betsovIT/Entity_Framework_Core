namespace P03_FootballBetting.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_FootballBetting.Data.Models;

    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {
        }

        public FootballBettingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Bet> Bets { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                builder.UseSqlServer(DataSetings.Connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Bet>(entity =>
            {
                entity
                    .HasKey(b => b.BetId);
                entity
                    .Property(b => b.Amount)
                    .IsRequired(true);
                entity
                    .HasOne(b => b.Game)
                    .WithMany(g => g.Bets)
                    .HasForeignKey(b => b.GameId);
                entity
                    .Property(b => b.Prediction)
                    .IsRequired(true)
                    .HasMaxLength(50);
                entity
                    .Property(b => b.DateTime)
                    .IsRequired(true);
                entity
                    .HasOne(b => b.User)
                    .WithMany(u => u.Bets)
                    .HasForeignKey(b => b.UserId);
            });

            builder.Entity<Color>(entity =>
            {
                entity
                    .HasKey(c => c.ColorId);
                entity
                    .Property(c => c.Name)
                    .HasMaxLength(20)
                    .IsRequired(true);
            });

            builder.Entity<Country>(entity =>
            {
                entity
                    .HasKey(c => c.CountryId);
                entity
                    .Property(c => c.Name)
                    .IsRequired(true)
                    .HasMaxLength(20);
            });

            builder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.GameId);
                entity.Property(g => g.AwayTeamBetRate).IsRequired(true);
                entity.Property(g => g.AwayTeamGoals).IsRequired(true);
                entity.HasOne(g => g.AwayTeam).WithMany(at => at.AwayGames).HasForeignKey(g => g.AwayTeamId);
                entity.Property(g => g.DrawBetRate).IsRequired(true);
                entity.Property(g => g.HomeTeamBetRate).IsRequired(true);
                entity.Property(g => g.HomeTeamGoals).IsRequired(true);
                entity.HasOne(g => g.HomeTeam).WithMany(ht => ht.HomeGames).HasForeignKey(g => g.HomeTeamId);
                entity.Property(g => g.Result).IsRequired(true).HasMaxLength(5);
                entity.Property(g => g.DateTime).IsRequired(true);
            });

            builder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);

                entity.Property(p => p.IsInjured).HasDefaultValue(false);
                entity.Property(p => p.Name).HasMaxLength(40).IsRequired(true).IsUnicode(true);
                entity.Property(p => p.SquadNumber).IsRequired(true);
                entity.HasOne(p => p.Position).WithMany(p => p.Players).HasForeignKey(p => p.PositionId);
                entity.HasOne(p => p.Team).WithMany(t => t.Players).HasForeignKey(p => p.TeamId);
            });

            builder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(ps => new { ps.GameId, ps.PlayerId });
                entity.HasOne(ps => ps.Game).WithMany(g => g.PlayerStatistics).HasForeignKey(ps => ps.GameId);
                entity.HasOne(ps => ps.Player).WithMany(p => p.PlayerStatistics).HasForeignKey(ps => ps.PlayerId);
                entity.Property(ps => ps.MinutesPlayed).IsRequired(true);
                entity.Property(ps => ps.ScoredGoals).IsRequired(true);
                entity.Property(ps => ps.Assists).IsRequired(true);
            });

            builder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.PositionId);
                entity.Property(p => p.Name).IsRequired(true).HasMaxLength(50).IsUnicode(true);
            });

            builder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.TeamId);
                entity.HasOne(t => t.Town).WithMany(t => t.Teams).HasForeignKey(t => t.TownId);
                entity.HasOne(t => t.PrimaryKitColor).WithMany(c => c.PrimaryKitTeams).HasForeignKey(t => t.PrimaryKitColorId);
                entity.HasOne(t => t.SecondaryKitColor).WithMany(c => c.SecondaryKitTeams).HasForeignKey(t => t.SecondaryKitColorId);
                entity.Property(t => t.Budget).IsRequired(true);
                entity.Property(t => t.Initials).HasMaxLength(3).IsRequired(true);
                entity.Property(t => t.LogoUrl).HasMaxLength(20).IsRequired(true);
                entity.Property(t => t.Name).HasMaxLength(50).IsUnicode(true).IsRequired(true);
            });

            builder.Entity<Town>(entity =>
            {
                entity.HasKey(t => t.TownId);
                entity.Property(t => t.Name).HasMaxLength(20).IsRequired(true).IsUnicode(true);
                entity.HasOne(t => t.Country).WithMany(c => c.Towns).HasForeignKey(t => t.CountryId);
            });

            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Balance).IsRequired(true);
                entity.Property(u => u.Email).HasMaxLength(30).IsRequired(true);
                entity.Property(u => u.Name).HasMaxLength(30).IsRequired(true).IsUnicode(true);
                entity.Property(u => u.Password).HasMaxLength(20).IsRequired(true).IsUnicode(false);
                entity.Property(u => u.Username).HasMaxLength(30).IsRequired(true).IsUnicode(false);

            });
        }
    }
}
