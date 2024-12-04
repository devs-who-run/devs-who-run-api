using DevsWhoRun.Api;
using DevsWhoRun.Api.Modules.Auth.DTOs;
using DevsWhoRun.Api.Modules.Auth.Services.Interfaces;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DevsWhoRun.UnitTests.Modules.Auth;
public class AuthModuleUnitTests
{
    private readonly IAuthService _authServiceMock = Substitute.For<IAuthService>();

    [Fact]
    public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = GivenRegisterRequest();
        var expectedResponse = new AuthResponse("token", "refreshToken", DateTime.Now, new MemberDTO(1,"firstname", "lastname", "test@example.com", "githubuser", "staravia", null, UserType.User));

        _authServiceMock.RegisterAsync(request).ReturnsForAnyArgs(Task.FromResult(expectedResponse));

        // Act
        var response = await _authServiceMock.RegisterAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var request = GivenRegisterRequest();

        _authServiceMock.RegisterAsync(request).Throws(new Exception("Registration failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authServiceMock.RegisterAsync(request));
        exception.Message.Should().Be("Registration failed");
    }

    private static RegisterRequest GivenRegisterRequest()
    {
        return new RegisterRequest("firstname", "lastname", "test@example.com", "Password123", "githubuser", "staravia");
    }
}
