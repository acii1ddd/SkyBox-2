using AutoMapper;
using SkyBox.API.Contracts.Users;
using SkyBox.Domain.Models;
using SkyBox.Domain.Models.User;

namespace SkyBox.API.Contracts.ContractProfiles;

public class UserContractProfile : Profile
{
    public UserContractProfile()
    {
        CreateMap<ShortUser, GetUserResponse>();
    }
}