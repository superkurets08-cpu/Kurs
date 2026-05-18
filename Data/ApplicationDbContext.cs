using KURS_ASP.NET.Models;
using Microsoft.EntityFrameworkCore;

namespace KURS_ASP.NET.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tournament> Tournaments => Set<Tournament>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<MatchDetail> MatchDetails => Set<MatchDetail>();
        public DbSet<RosterMembership> RosterMemberships => Set<RosterMembership>();
        public DbSet<Hero> Heroes => Set<Hero>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<TournamentComment> TournamentComments => Set<TournamentComment>();
        public DbSet<AdminActionLog> AdminActionLogs => Set<AdminActionLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team1)
                .WithMany(t => t.Matches1)
                .HasForeignKey(m => m.Team1ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team2)
                .WithMany(t => t.Matches2)
                .HasForeignKey(m => m.Team2ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.WinnerTeam)
                .WithMany(t => t.WonMatches)
                .HasForeignKey(m => m.WinnerTeamID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.CurrentTeam)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RosterMembership>()
                .HasOne(r => r.Player)
                .WithMany(p => p.RosterMemberships)
                .HasForeignKey(r => r.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RosterMembership>()
                .HasOne(r => r.Team)
                .WithMany(t => t.RosterMemberships)
                .HasForeignKey(r => r.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TournamentComment>()
                .HasOne(c => c.Tournament)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TournamentComment>()
                .HasOne(c => c.User)
                .WithMany(u => u.TournamentComments)
                .HasForeignKey(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AdminActionLog>()
                .HasOne(l => l.AdminUser)
                .WithMany(u => u.AdminActionLogs)
                .HasForeignKey(l => l.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tournament>()
                .Property(t => t.PrizePool)
                .HasPrecision(18, 6);

            modelBuilder.Entity<Hero>()
                .Property(h => h.StrengthGain)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Hero>()
                .Property(h => h.AgilityGain)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Hero>()
                .Property(h => h.IntelligenceGain)
                .HasPrecision(5, 2);

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => new { u.ExternalProvider, u.ExternalSubject })
                .IsUnique()
                .HasFilter("[ExternalProvider] IS NOT NULL AND [ExternalSubject] IS NOT NULL");

            modelBuilder.Entity<Player>()
                .HasIndex(p => p.TeamId);

            modelBuilder.Entity<RosterMembership>()
                .HasIndex(r => new { r.PlayerId, r.TeamId, r.StartYear });

            modelBuilder.Entity<TournamentComment>()
                .HasIndex(c => new { c.TournamentId, c.CreatedAtUtc });

            modelBuilder.Entity<AdminActionLog>()
                .HasIndex(l => l.CreatedAtUtc);
        }
    }
}
