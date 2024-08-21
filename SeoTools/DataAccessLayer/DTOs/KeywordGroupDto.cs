namespace DataAccessLayer.DTOs;

public class KeywordGroupDto
{
    public int Id { get; set; }

    public string GroupTitle { get; set; }
}

public class KeywordGroupReqDto
{
    public string GroupTitle { get; set; }
}