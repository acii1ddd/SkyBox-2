using SkyBox.BLL.Services.Users;
using SkyBox.Domain.Abstractions.Users;
using Xunit.Abstractions;

namespace SkyBox.BLL.Tests.User;

public class PasswordHashServiceTests
{   
    private readonly PasswordHashService _passwordHashService;
    private readonly ITestOutputHelper _testOutputHelper;

    public PasswordHashServiceTests(ITestOutputHelper testOutputHelper)
    {
        _passwordHashService = new PasswordHashService();
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