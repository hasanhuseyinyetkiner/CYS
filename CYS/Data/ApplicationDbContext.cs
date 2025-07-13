using Microsoft.EntityFrameworkCore;
using CYS.Models;

namespace CYS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing MySQL tables
        public DbSet<WeightMeasurement> WeightMeasurements { get; set; }
        public DbSet<WeaningWeightMeasurement> WeaningWeightMeasurements { get; set; }
        public DbSet<BirthWeightMeasurement> BirthWeightMeasurements { get; set; }
        
        // PostgreSQL sync tables - for storing data from client
        public DbSet<Animal> Animals { get; set; }
        public DbSet<AgirlikOlcum> AgirlikOlcums { get; set; }
        public DbSet<MilkAnalysisResult> MilkAnalysisResults { get; set; }
        public DbSet<MilkSample> MilkSamples { get; set; }
        public DbSet<MilkQualityAssessment> MilkQualityAssessments { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<SyncStatus> SyncStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<MilkAnalysisResult>()
                .HasOne(m => m.Animal)
                .WithMany(a => a.MilkAnalysisResults)
                .HasForeignKey(m => m.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MilkAnalysisResult>()
                .HasOne(m => m.Sample)
                .WithMany(s => s.AnalysisResults)
                .HasForeignKey(m => m.SampleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MilkSample>()
                .HasOne(s => s.Animal)
                .WithMany(a => a.MilkSamples)
                .HasForeignKey(s => s.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AgirlikOlcum>()
                .HasOne(a => a.Hayvan)
                .WithMany(h => h.WeightMeasurements)
                .HasForeignKey(a => a.hayvanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MilkQualityAssessment>()
                .HasOne(q => q.AnalysisResult)
                .WithMany()
                .HasForeignKey(q => q.AnalysisResultId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes for better performance
            modelBuilder.Entity<Animal>()
                .HasIndex(a => a.RfidTag)
                .IsUnique();

            modelBuilder.Entity<MilkSample>()
                .HasIndex(s => s.SampleId)
                .IsUnique();

            modelBuilder.Entity<MilkAnalysisResult>()
                .HasIndex(r => r.AnalysisDate);

            modelBuilder.Entity<AgirlikOlcum>()
                .HasIndex(a => a.OlcumTarihi);

            modelBuilder.Entity<SyncStatus>()
                .HasIndex(s => new { s.TableName, s.LastSyncTime });
        }
    }
} 