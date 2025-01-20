// using AutoFixture;
// using AutoMapper;
// using FluentAssertions;
// using SkyBox.DAL.Entities_dbDTOs_;
// using SkyBox.Domain.Models;
//
// namespace Tests;
//
// public class UserMappingTests
// {
//     private readonly IMapper _mapper;
//     private readonly IFixture _fixture;
//
//     public UserMappingTests()
//     {
//         var config = new MapperConfiguration(cfg => {});
//         _mapper = config.CreateMapper();
//         _fixture = new Fixture();
//     }
//
//     [Fact]
//     public void User_To_EntityUser()
//     {
//         var user = _fixture
//             .Build<User>()
//             .With(u => u.Files, new List<StorageFile>
//             {
//                 new StorageFile { Id = Guid.NewGuid(), Name = "file1.txt" },
//                 new StorageFile { Id = Guid.NewGuid(), Name = "file2.txt" }
//             })
//             .Create();
//         
//         // Act
//         var userEntity = _mapper.Map<UserEntity>(user);
//         
//         // Asserts
//         Assert.NotNull(userEntity);
//         Assert.Equal(user.Id, userEntity.Id);
//         Assert.Equal(user.UserName, userEntity.UserName);
//         Assert.Equal(user.Password, userEntity.Password);
//         Assert.Equal(user.Email, userEntity.Email);
//         userEntity.Files.Should().BeEquivalentTo(user.Files, options => 
//             options.ComparingByMembers<StorageFileEntity>());
//     }
// }