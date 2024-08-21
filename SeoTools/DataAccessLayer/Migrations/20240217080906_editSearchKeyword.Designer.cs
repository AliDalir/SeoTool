﻿// <auto-generated />
using System;
using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(SeoToolDbContext))]
    [Migration("20240217080906_editSearchKeyword")]
    partial class editSearchKeyword
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DataAccessLayer.DTOs.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.CrawlDate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CrawlDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("CrawlDates");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Keyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("KeywordGroupId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Query")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("KeywordGroupId");

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("GroupTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModificationDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("KeywordGroups");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordGroupHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("AllAvgPosition")
                        .HasColumnType("float");

                    b.Property<int>("CrawlDateId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("KeywordCount")
                        .HasColumnType("int");

                    b.Property<int>("KeywordGroupId")
                        .HasColumnType("int");

                    b.Property<double>("LastAvgPosition")
                        .HasColumnType("float");

                    b.Property<DateTime>("ModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("NoRankCount")
                        .HasColumnType("int");

                    b.Property<double>("Top100AvgPosition")
                        .HasColumnType("float");

                    b.Property<int>("Top100Count")
                        .HasColumnType("int");

                    b.Property<double>("Top10AvgPosition")
                        .HasColumnType("float");

                    b.Property<int>("Top10Count")
                        .HasColumnType("int");

                    b.Property<double>("Top3AvgPosition")
                        .HasColumnType("float");

                    b.Property<int>("Top3Count")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CrawlDateId");

                    b.HasIndex("KeywordGroupId");

                    b.ToTable("KeywordGroupHistories");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordHtml", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CrawlDateId")
                        .HasColumnType("int");

                    b.Property<string>("HtmlUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("KeywordId")
                        .HasColumnType("int");

                    b.Property<int>("SiteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("KeywordHtmls");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordSearchVolume", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("KeywordId")
                        .HasColumnType("int");

                    b.Property<string>("SearchVolume")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("KeywordId")
                        .IsUnique();

                    b.ToTable("KeywordSearchVolumes");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("KeywordId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("KeywordId");

                    b.HasIndex("TagId");

                    b.ToTable("KeywordTags");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordUrl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CrawlDateId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("KeywordId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CrawlDateId");

                    b.HasIndex("KeywordId");

                    b.ToTable("KeywordUrls");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Rank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CrawlDateId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("KeywordId")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<int>("SiteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CrawlDateId");

                    b.HasIndex("KeywordId");

                    b.HasIndex("SiteId");

                    b.ToTable("Ranks");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchConsoleDate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SearchConsoleDate");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchConsoleKeyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Query")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SearchConsoleKeyword");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchConsoleKeywordRank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Clicks")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ctr")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Impressions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SearchConsoleDateId")
                        .HasColumnType("int");

                    b.Property<int>("SearchConsoleKeywordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SearchConsoleDateId");

                    b.HasIndex("SearchConsoleKeywordId");

                    b.ToTable("SearchConsoleKeywordRank");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchConsoleReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("AvgPosition")
                        .HasColumnType("float");

                    b.Property<double>("Clicks")
                        .HasColumnType("float");

                    b.Property<double>("Ctr")
                        .HasColumnType("float");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Impressions")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("SearchConsoleReports");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchConsoleUrl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Page")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SearchConsoleKeywordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SearchConsoleKeywordId");

                    b.ToTable("SearchConsoleUrl");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchKeyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Query")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("Query")
                        .IsUnique();

                    b.ToTable("SearchKeywords");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchPerformance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float>("CTR")
                        .HasColumnType("real");

                    b.Property<int>("Clicks")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Impressions")
                        .HasColumnType("int");

                    b.Property<string>("Page")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Position")
                        .HasColumnType("real");

                    b.Property<int>("SearchKeywordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Date");

                    b.HasIndex("SearchKeywordId");

                    b.ToTable("SearchPerformances");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Site", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("KeywordGroupId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SiteName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SiteUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("KeywordGroupId");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModificationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Keyword", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.KeywordGroup", "KeywordGroup")
                        .WithMany("Keywords")
                        .HasForeignKey("KeywordGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KeywordGroup");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordGroupHistory", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.CrawlDate", "CrawlDate")
                        .WithMany("KeywordGroupHistories")
                        .HasForeignKey("CrawlDateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entites.KeywordGroup", "KeywordGroup")
                        .WithMany("KeywordGroupHistories")
                        .HasForeignKey("KeywordGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CrawlDate");

                    b.Navigation("KeywordGroup");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordSearchVolume", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.Keyword", "Keyword")
                        .WithOne("KeywordSearchVolume")
                        .HasForeignKey("DataAccessLayer.Entites.KeywordSearchVolume", "KeywordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Keyword");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordTag", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.Keyword", "Keyword")
                        .WithMany("KeywordTags")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entites.Tag", "Tag")
                        .WithMany("KeywordTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Keyword");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordUrl", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.CrawlDate", "CrawlDate")
                        .WithMany("KeywordUrls")
                        .HasForeignKey("CrawlDateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entites.Keyword", "Keyword")
                        .WithMany("KeywordUrls")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CrawlDate");

                    b.Navigation("Keyword");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Rank", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.CrawlDate", "CrawlDate")
                        .WithMany("Ranks")
                        .HasForeignKey("CrawlDateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entites.Keyword", "Keyword")
                        .WithMany("Ranks")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entites.Site", "Site")
                        .WithMany("Ranks")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CrawlDate");

                    b.Navigation("Keyword");

                    b.Navigation("Site");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchConsoleKeywordRank", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.SearchConsoleDate", "SearchConsoleDate")
                        .WithMany("SearchConsoleKeywordRanks")
                        .HasForeignKey("SearchConsoleDateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entites.SearchConsoleKeyword", "SearchConsoleKeyword")
                        .WithMany("SearchConsoleKeywordRanks")
                        .HasForeignKey("SearchConsoleKeywordId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SearchConsoleDate");

                    b.Navigation("SearchConsoleKeyword");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchConsoleUrl", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.SearchConsoleKeyword", "SearchConsoleKeyword")
                        .WithMany("SearchConsoleUrl")
                        .HasForeignKey("SearchConsoleKeywordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SearchConsoleKeyword");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchPerformance", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.SearchKeyword", "SearchKeyword")
                        .WithMany("SearchPerformances")
                        .HasForeignKey("SearchKeywordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SearchKeyword");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Site", b =>
                {
                    b.HasOne("DataAccessLayer.Entites.KeywordGroup", "KeywordGroup")
                        .WithMany("Sites")
                        .HasForeignKey("KeywordGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KeywordGroup");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.CrawlDate", b =>
                {
                    b.Navigation("KeywordGroupHistories");

                    b.Navigation("KeywordUrls");

                    b.Navigation("Ranks");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Keyword", b =>
                {
                    b.Navigation("KeywordSearchVolume")
                        .IsRequired();

                    b.Navigation("KeywordTags");

                    b.Navigation("KeywordUrls");

                    b.Navigation("Ranks");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.KeywordGroup", b =>
                {
                    b.Navigation("KeywordGroupHistories");

                    b.Navigation("Keywords");

                    b.Navigation("Sites");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchConsoleDate", b =>
                {
                    b.Navigation("SearchConsoleKeywordRanks");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchConsoleKeyword", b =>
                {
                    b.Navigation("SearchConsoleKeywordRanks");

                    b.Navigation("SearchConsoleUrl");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.SearchKeyword", b =>
                {
                    b.Navigation("SearchPerformances");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Site", b =>
                {
                    b.Navigation("Ranks");
                });

            modelBuilder.Entity("DataAccessLayer.Entites.Tag", b =>
                {
                    b.Navigation("KeywordTags");
                });
#pragma warning restore 612, 618
        }
    }
}
