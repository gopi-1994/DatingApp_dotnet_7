using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
           .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UsersParams usersParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != usersParams.CurrentUsername);
            query = query.Where(u=> u.Gender == usersParams.Gender);
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-usersParams.MaxAge-1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-usersParams.MinAge));
            
            query = query.Where(u=> u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

             query = usersParams.OrderBy switch
             {
                "created" => query.OrderByDescending(u=> u.Created),
                    _      => query.OrderByDescending(u=> u.LastActive)
             };

            return await PagedList<MemberDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider),
                usersParams.PageNumber,
                usersParams.PageSize); 
            
        }

        public async Task<IEnumerable<AppUser>> GetUserAsync() 
        {
            return await _context.Users
                    .Include(p => p.Photos)
                    .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users
                 .Include(p => p.Photos)
                 .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        // unpdates automataically in entity framework
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}