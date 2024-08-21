using DataAccessLayer.Entites;

namespace DataAccessLayer.DTOs;

public class SearchConsoleResponse
{
    public SearchConsoleKeywordDto Keyword { get; set; }

    public SearchConsoleKeywordRankDto Report { get; set; }
}