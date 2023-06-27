using Microsoft.EntityFrameworkCore;

namespace practice
{
    public class CurrencyDbContext : DbContext
    {
        public DbSet<Actual> Actual { get; set; }
        public DbSet<History> History { get; set; }

        public CurrencyDbContext()
        {
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CurrencyDB;Username=postgres;Password=789654123");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actual>(rate =>
            {
                rate.ToTable("Actual");

                rate.HasKey(a => a.CurrencyId);

                // Указываем соответствующие типы данных и настройки для каждого поля:
                rate.Property(a => a.CurrencyId).HasColumnName("CurrencyId");

                rate.Property(a => a.Date).HasColumnName("Date").HasColumnType("date");
                rate.Property(a => a.CurrencyScale).HasColumnName("CurrencyScale");
                rate.Property(a => a.CurrencyOfficialRate).HasColumnName("CurrencyOfficialRate");
                rate.Property(a => a.CurrencyName).HasColumnName("CurrencyName").HasMaxLength(100);
                rate.Property(a => a.CurrencyAbbreviation).HasColumnName("CurrencyAbbreviation").HasMaxLength(10);
            });

            modelBuilder.Entity<History>(rate =>
            {
                rate.ToTable("History");

                rate.HasKey(h => h.Id);

                // Указываем соответствующие типы данных и настройки для каждого поля:
                rate.Property(h => h.Id).HasColumnName("Id");
                rate.Property(h => h.CurrencyId).HasColumnName("CurrencyId");

                rate.Property(h => h.Date).HasColumnName("Date").HasColumnType("date"); 
                rate.Property(h => h.CurrencyScale).HasColumnName("CurrencyScale");
                rate.Property(h => h.CurrencyOfficialRate).HasColumnName("CurrencyOfficialRate");

                rate.HasOne(h => h.Actual).WithMany(a => a.Histories).HasForeignKey(h => h.CurrencyId);
            });

        }
    }
}
