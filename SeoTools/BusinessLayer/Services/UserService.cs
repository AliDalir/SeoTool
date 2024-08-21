using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BusinessLayer.Repositories;
using DataAccessLayer.Context;
using DataAccessLayer.DTOs;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UtilityLayer.Hashing;

namespace BusinessLayer.Services;

public class UserService : IUserService
{
    
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly SeoToolDbContext _context;
    private readonly IMapper _mapper;

    public UserService(IBackgroundJobClient backgroundJobs,
        IMemoryCache cache,
        IConfiguration configuration,
        SeoToolDbContext context,
        IMapper mapper)
    {
        _backgroundJobs = backgroundJobs;
        _cache = cache;
        _configuration = configuration;
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserResponse> AddUser(RegisterDto user)
    {
        var mappedUser = _mapper.Map<User>(user);

        mappedUser.Password = PasswordHash.GetHashPassword(mappedUser.Password);
        mappedUser.CreationDateTime = DateTime.Now;
        mappedUser.ModificationDateTime = mappedUser.CreationDateTime;
        mappedUser.IsActive = true;

        await _context.Users.AddAsync(mappedUser);

        await _context.SaveChangesAsync();

        return _mapper.Map<UserResponse>(mappedUser);
    }

    public async Task<UserDto<UserResponse>> LoginUser(LoginDto user)
    {

        var password = PasswordHash.GetHashPassword(user.Password);
        
        var existingUser = await _context.Users
            .Where(u => u.Password == password
                        && u.Email == user.Email)
            .FirstOrDefaultAsync();

        if (existingUser == null)
        {
            return new UserDto<UserResponse>()
            {
                Error = true,
                Message = "NOT-FOUND"
            };
        }

        else if (existingUser.IsActive == false)
        {
            return new UserDto<UserResponse>()
            {
                Error = true,
                Message = "NOT-ACTIVE"
            };
        }

        return new UserDto<UserResponse>()
        {
            Data = _mapper.Map<UserResponse>(existingUser),
            Token = GenerateJwtToken(existingUser),
            Message = "LOGIN",
            Error = false
        };

    }

    public string GenerateJwtToken(User user)
    {
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes
            (_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            }),
            Expires = DateTime.Now.AddDays(1),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        var stringToken = tokenHandler.WriteToken(token);
        return stringToken;
    }
}