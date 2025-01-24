using SkyBox.BLL.Services.Users;
using Xunit.Abstractions;

namespace SkyBox.BLL.Tests.User;

public class PasswordHashServiceTests
{   
    private readonly PasswordHashService _passwordHashService;
    private readonly ITestOutputHelper _testOutputHelper;

    public PasswordHashServiceTests(PasswordHashService passwordHashService, ITestOutputHelper testOutputHelper)
    {
        _passwordHashService = passwordHashService;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void GetHash_ShouldReturnHash()
    {
        var hash = _passwordHashService.HashPassword("123");
        
        Assert.NotNull(hash);
        _testOutputHelper.WriteLine(hash);
    }
}