using AutoMapper;
using AutoMapper.QueryableExtensions;
using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Helpers;
using CommunicationAPI.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace CommunicationAPI.Data
{

    public class UserRepo : IuserRepo
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepo(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<MemberDTO>> GetMemberAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUserName);
            query = query.Where(g => g.Gender == userParams.Gender);
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
            var result = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            result = userParams.OrderBy switch
            {
                "created" => result.OrderByDescending(u => u.Created),
                _ => result.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDTO>.CreateAsync(result.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider),
                   userParams.PageNumber, userParams.PageSize);
        }

        public async Task<MemberDTO> GetMemberByIdAsync(int id)
        {
            return await _context.Users
                  .Where(x => x.Id == id)
                  .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                  .SingleOrDefaultAsync();
        }

        public async Task<MemberDTO> GetMemberByNameAsync(string userName)
        {
            return await _context.Users
                  .Where(x => x.UserName == userName)
                  .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                  .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUserAsync()
        {
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByNameAsync(string userName)
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
