using Api.Services.Interfaces;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace Data {
    public class ApplicationDbContext : DbContext {
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<MeterReading> MeterReadings { get; set; }

        private readonly IConfiguration _configuration;
        private readonly ICsvService _csvService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration, ICsvService csvService) : base(options) {
            _configuration = configuration;
            _csvService = csvService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MeterReading>()
                .HasKey(mr => new { mr.AccountId, mr.MeterReadingDateTime });

            modelBuilder.Entity<MeterReading>()
                .HasOne(mr => mr.Account)
                .WithMany()
                .HasForeignKey(mr => mr.AccountId);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder) {
            var accounts = _csvService.Read<Account>(_configuration.GetConnectionString("ENERGYSEED"));
            modelBuilder.Entity<Account>().HasData(accounts.ToArray());
        }
    }
}