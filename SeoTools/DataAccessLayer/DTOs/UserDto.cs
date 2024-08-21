namespace DataAccessLayer.DTOs;

public class UserDto<T>
{
    public T? Data { get; set; }

    public string Token { get; set; }

    public string? Message { get; set; }

    public bool Error { get; set; }
    
}