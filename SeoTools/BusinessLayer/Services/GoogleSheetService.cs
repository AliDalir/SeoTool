using BusinessLayer.Repositories;
using Google.Apis.Sheets.v4;
using UtilityLayer.GoogleSheets;

namespace BusinessLayer.Services;

public class GoogleSheetService
{
    const string SPREADSHEET_ID = "1jkfASduuZNumBHU6Gg4AxCMbOUI_Agud8X5n5MpwvZA";
    const string SHEET_NAME = "Tags";
    
    SpreadsheetsResource.ValuesResource _googleSheetValues;

    public GoogleSheetService(GoogleSheetsHelper googleSheetsHelper)
    {
        _googleSheetValues = googleSheetsHelper.Service.Spreadsheets.Values;
    }
    
    public void CrawlGoogleSheet()
    {
        var range = $"{SHEET_NAME}!A:B";
        var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
        var response = request.Execute();
        var values = response.Values;
        var result = NmvGeneratorMapper.MapFromRangeData(values);

        var test = "test";
    }
}