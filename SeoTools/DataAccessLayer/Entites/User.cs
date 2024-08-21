using DataAccessLayer.Base;

namespace DataAccessLayer.DTOs;

public class User : BaseEntity
{
    public string UserName { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string FullName { get; set; }

    public bool IsActive { get; set; }
}