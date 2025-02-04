using AutoMapper;
using SkyBox.DAL.Entities_dbDTOs_;
using SkyBox.Domain.Models;
using SkyBox.Domain.Models.File;

namespace SkyBox.DAL.MappingProfiles;

public class StorageFileProfile : Profile
{
    public StorageFileProfile()
    {
        CreateMap<StorageFile, StorageFileEntity>()
            .ForMember(dest => dest.UserEntityId, 
                opt 
                    => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserEntity,
                opt 
                    => opt.MapFrom(src => src.ShortUser))

            // тут наоборот CreateMap<StorageFileEntity, StorageFile>
            .ReverseMap()
            .ForMember(dest => dest.UserId,  
                    opt
                    => opt.MapFrom(src => src.UserEntityId))
            .ForMember(dest => dest.ShortUser, 
                opt
                    => opt.MapFrom(src => src.UserEntity));
    }
}