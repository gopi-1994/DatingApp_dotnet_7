using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<Userlike> GetUserlike(int sourceUserId, int targetUserId);
        Task<AppUser> GetUserWithLIkes(int userId);
        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);

    }
}