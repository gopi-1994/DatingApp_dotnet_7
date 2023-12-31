using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Controllers;
public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenServices _tokenServices;

    public AccountController(DataContext context, ITokenServices tokenServices)
    {
        _context = context;
        _tokenServices = tokenServices;
    }
    [HttpPost("register")] // post : api/account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.UserName))
        {
            return BadRequest("UserName is taken");
        }
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = registerDTO.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDTO
        {
            Username = user.UserName,
            Token = _tokenServices.CreateToken(user)
        };

    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {// verifyes username witjh thet db fro login
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);
        if (user == null) { return Unauthorized("Invalid Username"); }
        // verifyes password with hash in db to the input password converted hash
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) { return Unauthorized("Invalid Password"); }
        }
        return new UserDTO
        {
            Username = user.UserName,
            Token = _tokenServices.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

}
