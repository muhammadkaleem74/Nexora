using Nexora.Models.TokenAuth;
using Nexora.Web.Controllers;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Nexora.Web.Tests.Controllers;

public class HomeController_Tests : NexoraWebTestBase
{
    [Fact]
    public async Task Index_Test()
    {
        await AuthenticateAsync(null, new AuthenticateModel
        {
            UserNameOrEmailAddress = "admin",
            Password = "123qwe"
        });

        //Act
        var response = await GetResponseAsStringAsync(
            GetUrl<HomeController>(nameof(HomeController.Index))
        );

        //Assert
        response.ShouldNotBeNullOrEmpty();
    }
}