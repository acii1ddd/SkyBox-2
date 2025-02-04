using AutoMapper;
using SkyBox.DAL.Entities_dbDTOs_;
using SkyBox.Domain.Models.User;

namespace SkyBox.DAL.MappingProfiles.User;

public class UserJwtDetailsProfile : Profile
{
    public UserJwtDetailsProfile()
    {
        CreateMap<UserEntity, UserJwtDetails>()
            .ForMember(dest => dest.UserId, opt
                => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserRole, opt
                => opt.MapFrom(src => src.Role));
    }
}