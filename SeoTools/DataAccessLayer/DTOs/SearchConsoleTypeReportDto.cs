using DataAccessLayer.Entites;

namespace DataAccessLayer.DTOs;

public class SearchConsoleTypeReportDto
{
    public string StartDate { get; set; }
    public string EndDate { get; set; }

    public IEnumerable<SearchConsoleType> report { get; set; }
    
}

public class SearchConsoleType
{
    public string TypeName { get; set; }
    
    public SearchConsoleReport Type { get; set; }
}