using System.Text.Json.Serialization;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using AutoMapper;
using BusinessLayer.Caching;
using BusinessLayer.Elasticsearch;
using BusinessLayer.MongoDb;
using BusinessLayer.Profiles;
using BusinessLayer.Repositories;
using BusinessLayer.Services;
using DataAccessLayer.Context;
using DataAccessLayer.DTOs;
using Hangfire;
using MongoDB.Driver;
using StackExchange.Redis;
using UtilityLayer.GoogleSheets;


namespace SeoTools;

public static class ServiceRegistery
{
    public static IServiceCollection AddServiceRegistery(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
        });

        builder.Services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

        builder
            .Services
            .Configure<MongoDbConnectionSettings>(
                builder.Configuration.GetSection("MongoDbConnectionSettings")
            );
        
        builder.Services.AddSingleton<IMongoClient>(_ => {
            var connectionString = 
                builder
                    .Configuration
                    .GetSection("MongoDbConnectionSettings:ConnectionString")?
                    .Value;
            return new MongoClient(connectionString);
        });
        
        
        // builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer
        //     .Connect("209.38.208.107:6379"));
        //
        // builder.Services.AddRedisOutputCache();
        
        
        // builder.Services.AddOutputCache(options =>
        // {
        //     // Add a base policy that applies to all endpoints
        //     options.AddBasePolicy(basePolicy => basePolicy.Expire(TimeSpan.FromSeconds(120)));
        //
        //     // Add a named policy that applies to selected endpoints
        //     options.AddPolicy("Expire20", policyBuilder => policyBuilder.Expire(TimeSpan.FromSeconds(20)));
        // });


        builder.Services.AddStackExchangeRedisCache(redisOptions =>
        {
            string connection = builder.Configuration.GetConnectionString("Redis");
            redisOptions.Configuration = connection;
        });
        
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient();
        builder.Services.AddMemoryCache();
        builder.Services.AddHangfireServer();
        builder.Services.AddElasticSearch(builder.Configuration);
        
        

        return builder.Services;
    }


    public static IServiceCollection AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        // Auto Mapper Configurations

        #region Auto mapper

        var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });

        var mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

        #endregion


        #region Dependency Injection

        builder.Services.AddScoped<KeywordService>();
        builder.Services.AddScoped<IKeywordService, CachedKeywordService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddSingleton(typeof(GoogleSheetsHelper));
        builder.Services.AddScoped<IGoogleSearchConsoleService, GoogleSearchConsoleService>();
        builder.Services.AddScoped<ISyncDataService, SyncDataService>();
        builder.Services.AddScoped<IElasticService<KeywordELK>, ElasticService<KeywordELK>>();
        builder.Services.AddScoped<IElasticService<ReportDto>, ElasticService<ReportDto>>();
        builder.Services.AddScoped<ISearchService<KeywordELK>, SearchService<KeywordELK>>();
        builder.Services.AddScoped<IReportService, ReportService>();
        builder.Services.AddScoped<IHtmlService, HtmlService>();
        builder.Services.AddScoped<IS3Service, S3Service>();
        builder.Services.AddScoped<IViewService, ViewService>();
        
        
        var awsOptions = builder.Configuration.GetAWSOptions();
        awsOptions.Credentials = new Amazon.Runtime.BasicAWSCredentials(
            builder.Configuration["AWS:AccessKey"], 
            builder.Configuration["AWS:SecretKey"]);

        AmazonS3Config s3Config = new AmazonS3Config
        {
            ServiceURL = builder.Configuration["AWS:ServiceURL"],
            UseHttp = true,
            ForcePathStyle = true // Required for S3 compatible services like DigitalOcean Spaces
        };

        builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsOptions.Credentials, s3Config));

        #endregion

        #region Db Connection

        builder.Services.AddDbContext<SeoToolDbContext>();
        
        #endregion

        return builder.Services;
    }
}