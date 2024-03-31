using System.Security.Claims;
using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
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
    private readonly IphotoService _photoService;

    public UsersController(IUserRepository userRepository, IMapper mapper, IphotoService photoService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _photoService = photoService;
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
        var username = User.GetUsername();
        // gets updates from input by user and stores in this variable
        var user = await _userRepository.GetUserByUserNameAsync(username);

        if (user == null) return NotFound();
        // updates the info in db using Mapper
        _mapper.Map(memberUpdateDTO,user);
        if (await _userRepository.SaveAllAsync()) return NoContent(); // will send 204 response code if data is updated successfully
        // if no changes made it will send 504 error
    
        return BadRequest("Failed to update user");
    }
    [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file){
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            if (user == null)
            {
                return NotFound();
            }
            var result =  await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(user.Photos.Count==0) photo.IsMain = true;
            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync()) 
            {
                return CreatedAtAction(nameof(GetUser),
                    new {username = user.UserName},
                     _mapper.Map<PhotoDTO>(photo));
        }
            
            return BadRequest("Problem Adding photo");

        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            if (user== null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x=> x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("This is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x=> x.IsMain);
            if (currentMain !=null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await _userRepository.SaveAllAsync()) return NoContent();
            
            return BadRequest("");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x=> x.Id == photoId);
            if(photo == null ) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");
            if (photo.PublicId != null){
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if  (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Problem deleting photo");
        }



}
