// using Xunit.Abstractions;
//
// namespace SkyBox.BLL.Tests.File;
//
// public class GetSharedUrlAsyncTests
// {
//     private readonly ITestOutputHelper _testOutputHelper;
//     private HttpClient _httpClient = new();
//
//     public GetSharedUrlAsyncTests(ITestOutputHelper testOutputHelper)
//     {
//         _testOutputHelper = testOutputHelper;
//     }
//
//     [Fact]
//     public async Task GetSharedUrlTest_ShouldBeValid()
//     {
//         const string url = "https://localhost:9000/user-files/387a29db-2975-4882-a61e-e8e332a74041/2d7b272e-0dbe-4554-929f-b0a0012001de?AWSAccessKeyId=user-user&Expires=1738433192&Signature=mWgxDGf3jIYIuDknGI60z6otskY%3D";
//
//         try
//         {
//             var result = await _httpClient.GetAsync(url);
//             
//             // Asserts
//             Assert.True(result.IsSuccessStatusCode);
//             var content = await result.Content.ReadAsStringAsync();
//             
//             Assert.NotEmpty(content);
//         }
//         catch (Exception e)
//         {
//             _testOutputHelper.WriteLine(e.ToString());
//             throw;
//         }
//     }
// }