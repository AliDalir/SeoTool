using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace UtilityLayer.GoogleSheets;

public class GoogleSheetsHelper
{
    public SheetsService Service { get; set; }
    const string APPLICATION_NAME = "SeoTools";
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    public GoogleSheetsHelper()
    {
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
        using (var stream = new FileStream("digikalacom-90d6e72d64c9.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }
        return credential;
    }
}