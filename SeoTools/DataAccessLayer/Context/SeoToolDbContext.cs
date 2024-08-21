using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Context;

public class SeoToolDbContext : DbContext
{
    public SeoToolDbContext(DbContextOptions<SeoToolDbContext> options) : base(options)
    {
        Database.SetCommandTimeout(120);
    }


    public DbSet<Keyword> Keywords { get; set; }

    public DbSet<Rank> Ranks { get; set; }
    public DbSet<Site> Sites { get; set; }

    public DbSet<KeywordGroup> KeywordGroups { get; set; }

    public DbSet<CrawlDate> CrawlDates { get; set; }

    public DbSet<KeywordGroupHistory> KeywordGroupHistories { get; set; }

    public DbSet<KeywordUrl> KeywordUrls { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<KeywordTag> KeywordTags { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<KeywordSearchVolume> KeywordSearchVolumes { get; set; }
    
    public DbSet<KeywordHtml> KeywordHtmls { get; set; }

    public DbSet<SearchConsoleReport> SearchConsoleReports { get; set; }
    
    public DbSet<SearchKeyword> SearchKeywords { get; set; }
    
    public DbSet<SearchPerformance> SearchPerformances { get; set; }

    public DbSet<CompetitorsSummery> CompetitorsSummeries { get; set; }

    public DbSet<ExcelExport> ExcelExports { get; set; }


    public DbSet<View> Views { get; set; }

    public DbSet<KeywordInView> KeywordInViews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(
            "Data Source=164.92.229.6,1433; Initial Catalog=seotools; User Id=sa; Password=T4W9LJC537d2TJ6d;TrustServerCertificate=true;",
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10, // Maximum number of retries
                    maxRetryDelay: TimeSpan.FromSeconds(30), // Maximum delay between retries
                    errorNumbersToAdd: null // SQL error numbers to consider for retry; null defaults to SQL Server's transient errors
                );
            });
        
        optionsBuilder.EnableSensitiveDataLogging();

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Rank>()
            .HasOne(r => r.Site)
            .WithMany(s => s.Ranks)
            .HasForeignKey(r => r.SiteId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<SearchConsoleKeywordRank>()
            .HasOne(rank => rank.SearchConsoleKeyword)
            .WithMany(keyword => keyword.SearchConsoleKeywordRanks)
            .HasForeignKey(rank => rank.SearchConsoleKeywordId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        // Set the indexes
        modelBuilder.Entity<SearchKeyword>()
            .HasIndex(p => p.Id); // Although KeywordID is a primary key and indexed by default, included here for completeness
        
        modelBuilder.Entity<SearchKeyword>()
            .HasIndex(p => p.Query)
            .IsUnique();

        modelBuilder.Entity<SearchPerformance>()
            .HasIndex(p => p.SearchKeywordId);

        modelBuilder.Entity<SearchPerformance>()
            .HasIndex(p => p.Date);

        modelBuilder.Entity<CompetitorsSummery>()
            .HasIndex(c => c.SiteId);

        modelBuilder.Entity<Site>()
            .HasIndex(s => s.SiteName);
            
        modelBuilder.Entity<Site>()
            .HasIndex(s => s.SiteUrl);
        
        modelBuilder.Entity<Rank>()
            .HasIndex(r => r.KeywordId);       
        
        modelBuilder.Entity<Rank>()
            .HasIndex(r => r.CrawlDateId);
        
        modelBuilder.Entity<Rank>()
            .HasIndex(r => r.Position);
        
        modelBuilder.Entity<View>()
            .HasIndex(r => r.ViewName);
        
        modelBuilder.Entity<KeywordInView>()
            .HasIndex(r => r.ViewId);

    }
}