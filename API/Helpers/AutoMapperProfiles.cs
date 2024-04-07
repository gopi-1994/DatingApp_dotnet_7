using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>()
            .ForMember(destinationMember => destinationMember.PhotoUrl,
                opt => opt.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest=> dest.Age, opt => opt.MapFrom(source => source.DateOfBirth.CalculateAge()));    
            CreateMap<Photo, PhotoDTO>();
            CreateMap<MemberUpdateDTO, AppUser> ();
            CreateMap<RegisterDTO, AppUser>();// registerDTO 
        }
    } 
}