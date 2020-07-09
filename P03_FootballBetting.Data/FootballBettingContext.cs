using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Models;
using P03_FootballBetting.Data.Config;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {
        }

        public FootballBettingContext(DbContextOptions options)
            : base(options)
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Connection.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.TeamId);

                entity
                    .Property(t => t.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(50);

                entity
                    .Property(t => t.LogoUrl)
                    .IsRequired()
                    .IsUnicode(false);

                entity
                    .Property(t => t.Initials)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(3);

                entity
                    .HasOne(t => t.PrimaryKitColor)
                    .WithMany(c => c.PrimaryKitTeams)
                    .HasForeignKey(t => t.PrimaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(t => t.SecondaryKitColor)
                    .WithMany(c => c.SecondaryKitTeams)
                    .HasForeignKey(t => t.SecondaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(team => team.Town)
                    .WithMany(town => town.Teams)
                    .HasForeignKey(team => team.TownId);
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(c => c.ColorId);

                entity
                    .Property(c => c.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Town>(entity =>
            {
                entity.HasKey(t => t.TownId);

                entity
                    .Property(t => t.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(50);

                entity
                    .HasOne(t => t.Country)
                    .WithMany(c => c.Towns)
                    .HasForeignKey(t => t.CountryId);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(c => c.CountryId);

                entity
                    .Property(c => c.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);

                entity
                    .Property(p => p.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(80);

                entity
                    .Property(p => p.SquadNumber)
                    .IsRequired();

                entity
                    .HasOne(p => p.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TeamId);

                entity
                    .HasOne(player => player.Position)
                    .WithMany(position => position.Players)
                    .HasForeignKey(player => player.PositionId);

                entity
                    .Property(p => p.IsInjured)
                    .IsRequired();
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.PositionId);

                entity
                    .Property(p => p.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(ps => new { ps.GameId, ps.PlayerId });

                entity
                    .Property(ps => ps.ScoredGoals)
                    .IsRequired();

                entity
                    .Property(ps => ps.Assists)
                    .IsRequired();

                entity
                    .Property(ps => ps.MinutesPlayed)
                    .IsRequired();

                entity
                    .HasOne(ps => ps.Game)
                    .WithMany(g => g.PlayerStatistics)
                    .HasForeignKey(ps => ps.GameId);

                entity
                    .HasOne(ps => ps.Player)
                    .WithMany(player => player.PlayerStatistics)
                    .HasForeignKey(ps => ps.PlayerId);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.GameId);

                entity
                    .HasOne(g => g.HomeTeam)
                    .WithMany(t => t.HomeGames)
                    .HasForeignKey(g => g.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(g => g.AwayTeam)
                    .WithMany(t => t.AwayGames)
                    .HasForeignKey(g => g.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .Property(g => g.HomeTeamGoals)
                    .IsRequired();

                entity
                    .Property(g => g.AwayTeamGoals)
                    .IsRequired();

                entity
                    .Property(g => g.DateTime)
                    .IsRequired();

                entity
                    .Property(g => g.HomeTeamBetRate)
                    .IsRequired();

                entity
                    .Property(g => g.AwayTeamBetRate)
                    .IsRequired();

                entity
                    .Property(g => g.DrawBetRate)
                    .IsRequired();

                entity
                    .Property(g => g.Result)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<Bet>(entity =>
            {
                entity.HasKey(b => b.BetId);

                entity
                    .Property(b => b.Amount)
                    .IsRequired();

                entity
                    .Property(b => b.Prediction)
                    .IsRequired();

                entity
                    .Property(b => b.DateTime)
                    .IsRequired();

                entity
                    .HasOne(b => b.Game)
                    .WithMany(g => g.Bets)
                    .HasForeignKey(b => b.GameId);

                entity
                    .HasOne(b => b.User)
                    .WithMany(u => u.Bets)
                    .HasForeignKey(b => b.UserId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity
                    .Property(u => u.Username)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(50);

                entity
                    .Property(u => u.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(80);

                entity
                    .Property(u => u.Password)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasMaxLength(256);

                entity
                    .Property(u => u.Email)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasMaxLength(100);
            });
        }
    }
}
