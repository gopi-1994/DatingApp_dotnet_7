using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Controllers;
public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenServices _tokenServices;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenServices tokenServices, IMapper mapper)
    {   
        _context = context;
        _tokenServices = tokenServices;
        _mapper = mapper;
    }
    [HttpPost("register")] // post : api/account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.UserName))
        {
            return BadRequest("UserName is taken");
        }
        var user = _mapper.Map<AppUser>(registerDTO);
        using var hmac = new HMACSHA512();
            user.UserName = registerDTO.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            user.PasswordSalt = hmac.Key;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDTO
        {
            Username = user.UserName,
            Token = _tokenServices.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };

    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {// verifyes username witjh thet db fro login
        var user = await _context.Users
                        .Include(p=> p.Photos)
                        .SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);
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
            Token = _tokenServices.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=> x.IsMain).Url,
            KnownAs = user.KnownAs,
            Gender =  user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

}
