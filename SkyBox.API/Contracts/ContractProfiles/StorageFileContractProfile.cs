using AutoMapper;
using SkyBox.API.Contracts.StorageFiles;
using SkyBox.Domain.Models;
using SkyBox.Domain.Models.File;

namespace SkyBox.API.Contracts.ContractProfiles;

public class StorageFileContractProfile : Profile
{
    public StorageFileContractProfile()
    {
        CreateMap<StorageFile, GetStorageFileResponse>();
    }
}