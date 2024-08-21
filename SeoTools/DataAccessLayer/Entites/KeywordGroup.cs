using DataAccessLayer.Base;

namespace DataAccessLayer.Entites;

public class KeywordGroup : BaseEntity
{
    public string GroupTitle { get; set; }


    public List<Keyword> Keywords { get; set; }
    public List<Site> Sites { get; set; }

    public List<KeywordGroupHistory> KeywordGroupHistories { get; set; }
    public List<ExcelExport> ExcelExports { get; set; }
}