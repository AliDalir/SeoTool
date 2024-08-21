using BusinessLayer.Repositories;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace SeoTools.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{

    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("RegisterUser")]
    public async Task<UserResponse> RegisterUser(RegisterDto user)
    {
        return await _userService.AddUser(user);
    }
    
    [HttpGet("LoginUser")]
    public async Task<UserDto<UserResponse>> LoginUser(string email, string password)
    {
        return await _userService.LoginUser(new LoginDto()
        {
            Email = email,
            Password = password
        });
    }
}