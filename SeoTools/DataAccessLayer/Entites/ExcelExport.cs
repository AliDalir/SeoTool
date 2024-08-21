namespace DataAccessLayer.Entites;

public class ExcelExport
{
    public int Id { get; set; }

    public int KeywordGroupId { get; set; }

    public string FilePath { get; set; }


    public KeywordGroup KeywordGroup { get; set; }
}