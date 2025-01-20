// using AutoMapper;
// using SkyBox.API.Contracts.ContractProfiles;
// using SkyBox.DAL.MappingProfiles;
//
// namespace Tests;
//
// public class StorageFileMappingTests
// {
//     private readonly IMapper _mapper;
//
//     public StorageFileMappingTests()
//     {
//         var config = new MapperConfiguration(cfg =>
//         {
//             cfg.AddProfile(typeof(StorageFileProfile));
//             cfg.AddProfile(typeof(StorageFileContractProfile));
//         });
//         
//         _mapper = config.CreateMapper();
//     }
//
//     [Fact]
//     public void Test1()
//     {
//         
//     }
// }