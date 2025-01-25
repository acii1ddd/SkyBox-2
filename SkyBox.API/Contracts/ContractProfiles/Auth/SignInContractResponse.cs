using AutoMapper;
using SkyBox.API.Contracts.Auth;
using SkyBox.Domain.Models.Auth;

namespace SkyBox.API.Contracts.ContractProfiles.Auth;

public class SignInContractResponse : Profile
{
    public SignInContractResponse()
    {
        CreateMap<AuthTokenModel, SignInResponse>();
    }
}