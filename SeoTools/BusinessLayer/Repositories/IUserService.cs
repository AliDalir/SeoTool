using DataAccessLayer.DTOs;

namespace BusinessLayer.Repositories;

public interface IUserService
{
    public Task<UserResponse> AddUser(RegisterDto user);
    
    public Task<UserDto<UserResponse>> LoginUser(LoginDto user);

    public string GenerateJwtToken(User user);
}