namespace DataAccessLayer.Entites;

public class PaginitionResDto<T>
{
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public T Data { get; set; }
}