using System.Security.Claims;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    // api/users 
    // gets list of all users
    // [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
    {
        var users = await _userRepository.GetMembersAsync();
        // var usersToReturn = _mapper.Map<IEnumerable<MemberDTO>>(users);
        return Ok(users);

    }
    //api/users/kim
    [HttpGet("{userName}")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        return await _userRepository.GetMemberAsync(username);
        // return _mapper.Map<MemberDTO>(user);
    }
    //api/users/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // gets updates from input by user and stores in this variable
        var user = await _userRepository.GetUserByUserNameAsync(username);

        if (user == null) return NotFound();
        // updates the info in db using Mapper
        _mapper.Map(memberUpdateDTO,user);
        if (await _userRepository.SaveAllAsync()) return NoContent(); // will send 204 response code if data is updated successfully
        // if no changes made it will send 504 error
    
        return BadRequest("Failed to update user");
    }

}
