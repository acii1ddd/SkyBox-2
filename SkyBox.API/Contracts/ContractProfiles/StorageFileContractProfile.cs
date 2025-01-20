using AutoMapper;
using SkyBox.API.Contracts.StorageFiles;
using SkyBox.Domain.Models;

namespace SkyBox.API.Contracts.ContractProfiles;

public class StorageFileContractProfile : Profile
{
    public StorageFileContractProfile()
    {
        CreateMap<StorageFile, GetStorageFileResponse>();
    }
}