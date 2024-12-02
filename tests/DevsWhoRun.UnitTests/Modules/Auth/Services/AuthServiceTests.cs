using DevsWhoRun.Api;
using DevsWhoRun.Api.Modules.Auth.DTOs;
using DevsWhoRun.Api.Modules.Auth.Services.Interfaces;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DevsWhoRun.UnitTests.Modules.Auth.Services;

public class AuthServiceTests
{
    private readonly IAuthService _authServiceMock = Substitute.For<IAuthService>();

    [Fact]
    public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
    {
        var request = GivenRegisterRequest();
        var expectedResponse = new AuthResponse(
            "token",
            "refreshToken",
            DateTime.Now.AddHours(1),
            new MemberDTO(1, "firstname", "lastname", "test@example.com", "githubuser", "staravia", null, UserType.User)
        );

        _authServiceMock.RegisterAsync(request).Returns(Task.FromResult(expectedResponse));

        var response = await _authServiceMock.RegisterAsync(request);

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailAlreadyRegistered()
    {
        var request = GivenRegisterRequest();
        _authServiceMock
            .RegisterAsync(request)
            .Throws(new Exception("Email already registered"));

        var exception = await Assert.ThrowsAsync<Exception>(() => _authServiceMock.RegisterAsync(request));
        exception.Message.Should().Be("Email already registered");
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
    {
        var request = new LoginRequest("test@example.com", "Password123");
        var expectedResponse = new AuthResponse(
            "token",
            "refreshToken",
            DateTime.Now.AddHours(1),
            new MemberDTO(1, "firstname", "lastname", "test@example.com", "githubuser", "staravia", null, UserType.User)
        );

        _authServiceMock.LoginAsync(request).Returns(Task.FromResult(expectedResponse));

        var response = await _authServiceMock.LoginAsync(request);

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Login_ThrowsException_WhenInvalidCredentialsProvided()
    {
        var request = new LoginRequest("test@example.com", "WrongPassword");

        _authServiceMock
            .LoginAsync(request)
            .Throws(new Exception("Invalid credentials"));

        var exception = await Assert.ThrowsAsync<Exception>(() => _authServiceMock.LoginAsync(request));
        exception.Message.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task RefreshToken_ReturnsOk_WhenTokenIsValid()
    {
        var validRefreshToken = "validRefreshToken";
        var expectedResponse = new AuthResponse(
            "newToken",
            "newRefreshToken",
            DateTime.Now.AddHours(1),
            new MemberDTO(1, "firstname", "lastname", "test@example.com", "githubuser", "staravia", null, UserType.User)
        );

        _authServiceMock.RefreshTokenAsync(validRefreshToken).Returns(Task.FromResult(expectedResponse));

        var response = await _authServiceMock.RefreshTokenAsync(validRefreshToken);

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task RefreshToken_ThrowsException_WhenTokenIsInvalidOrExpired()
    {
        var invalidRefreshToken = "invalidRefreshToken";

        _authServiceMock
            .RefreshTokenAsync(invalidRefreshToken)
            .Throws(new Exception("Invalid or expired refresh token"));

        var exception = await Assert.ThrowsAsync<Exception>(() => _authServiceMock.RefreshTokenAsync(invalidRefreshToken));
        exception.Message.Should().Be("Invalid or expired refresh token");
    }

    private static RegisterRequest GivenRegisterRequest()
    {
        return new RegisterRequest(
            "firstname",
            "lastname",
            "test@example.com",
            "Password123",
            "githubuser",
            "staravia"
        );
    }
}
