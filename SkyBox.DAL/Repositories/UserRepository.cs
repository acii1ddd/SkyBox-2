using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SkyBox.DAL.Context;
using SkyBox.Domain.Abstractions.Users;
using SkyBox.Domain.Models.User;

namespace SkyBox.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    
    public UserRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // UserName should be unique
    public async Task<UserSignInDetails?> GetByUserNameAsync(string userName)
    {
        return _mapper.Map<UserSignInDetails>(await _context.Users
            .AsNoTracking()
            .Where(x => x.UserName == userName)
            .FirstOrDefaultAsync()
        );
    }
}