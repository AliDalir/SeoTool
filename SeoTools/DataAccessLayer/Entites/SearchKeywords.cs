using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entites;

public class SearchKeyword
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    
    [Required]
    public string Query { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<SearchPerformance> SearchPerformances { get; set; }
}