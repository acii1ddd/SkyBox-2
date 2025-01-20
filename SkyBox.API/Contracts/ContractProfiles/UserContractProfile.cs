using AutoMapper;
using SkyBox.API.Contracts.Users;
using SkyBox.Domain.Models;

namespace SkyBox.API.Contracts.ContractProfiles;

public class UserContractProfile : Profile
{
    public UserContractProfile()
    {
        CreateMap<User, GetUserResponse>();
    }
}