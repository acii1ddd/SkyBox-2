using AutoMapper;
using SkyBox.DAL.Entities_dbDTOs_;
using SkyBox.Domain.Models.User;

namespace SkyBox.DAL.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserSignInDetails>()
            .ForMember(dest => dest.UserId, opt 
                => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserRole, opt 
            => opt.MapFrom(src => src.Role));
    }
}