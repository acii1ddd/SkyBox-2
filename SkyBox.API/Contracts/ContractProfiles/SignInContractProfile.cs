using AutoMapper;
using SkyBox.API.Contracts.Auth;
using SkyBox.Domain.Models.User;

namespace SkyBox.API.Contracts.ContractProfiles;

public class SignInContractProfile : Profile
{
    public SignInContractProfile()
    {
        CreateMap<SignInRequest, SignInModel>();
    }
}