using BusinessLayer.Repositories;
using BusinessLayer.Services;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Mvc;
using UtilityLayer.GoogleSheets;

namespace SeoTools.Controllers;

public class GoogleSheetsController : Controller
{

    const string SPREADSHEET_ID = "1jkfASduuZNumBHU6Gg4AxCMbOUI_Agud8X5n5MpwvZA";
    const string SHEET_NAME = "Competitor";
    
    SpreadsheetsResource.ValuesResource _googleSheetValues;

    private readonly IKeywordService _keywordService;

    public GoogleSheetsController(GoogleSheetsHelper googleSheetsHelper, IKeywordService keywordService)
    {
        _keywordService = keywordService;
        _googleSheetValues = googleSheetsHelper.Service.Spreadsheets.Values;
    }
    

    // GET
    // [HttpGet("GetKeywords")]
    // public async Task GetKeywords()
    // {
    //     var range = $"{SHEET_NAME}!A:B";
    //     var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
    //     var response = request.Execute();
    //     var values = response.Values;
    //     var results = NmvGeneratorMapper.MapFromRangeData(values);
    //     
    //     
    //     foreach (var result in results)
    //     {
    //         var keyword = new KeywordDto()
    //         {
    //             Query = result.Keyword,
    //             KeywordGroupId = 6
    //         };
    //
    //         await _keywordService.AddKeywordAsync(keyword);
    //     }
    //     
    //     
    //     var test = "test";
    // }
    
    
    
    
    [HttpGet("AddVerticalsWithCompetitors")]
    public async Task AddVerticalsWithCompetitors(string sheetName,string spreadsheetId)
    {
        var range = $"{sheetName}!A:B";
        var request = _googleSheetValues.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        var results = VerticalWithCompetitorsMapper.MapFromRangeData(values);


        await _keywordService.AddVerticalsWithCompetitors(results);
    }
    
    
    [HttpGet("AddKeywordsWithTags")]
    public async Task AddKeywordsWithTags(string sheetName,string spreadsheetId)
    {
        var range = $"{sheetName}!A:B";
        var request = _googleSheetValues.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        var results = KeywordWithTagMapper.MapFromRangeData(values);

        await _keywordService.AddKeywordsWithTags(results);
    }
    
}