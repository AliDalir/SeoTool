using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class KeywordDto
{
    [Required] public string Query { get; set; }
    
    public int KeywordGroupId { get; set; }
}