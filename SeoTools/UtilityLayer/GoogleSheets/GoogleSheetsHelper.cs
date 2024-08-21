using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Configuration;

namespace UtilityLayer.GoogleSheets;

public class GoogleSheetsHelper
{
    public SheetsService Service { get; set; }
    const string APPLICATION_NAME = "SeoTools";
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private readonly IConfiguration _configuration; 
    public GoogleSheetsHelper(IConfiguration configuration)
    {
        _configuration = configuration;
        InitializeService();
    }
    private void InitializeService()
    {
        var credential = GetCredentialsFromFile();
        Service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = APPLICATION_NAME
        });
    }
    private GoogleCredential GetCredentialsFromFile()
    {
        GoogleCredential credential;
        using (var stream = new FileStream(_configuration.GetSection("creds").ToString(), FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }
        return credential;
    }
}