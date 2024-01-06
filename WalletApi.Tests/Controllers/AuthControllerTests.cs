using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WalletWebApi.Controllers;
using WalletWebApi.Models.Domain;
using WalletWebApi.Models.Dto;
using WalletWebApi.Repositories;

namespace WalletApi.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IUserStore<ApplicationUser>> _storeMock = new();
    
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITokenRepository> _tokenRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<AuthController>> _loggerMock = new();

    private readonly AuthController _controller;
    
    public AuthControllerTests()
    
    {
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            _storeMock.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "13a975c3-d157-4443-b900-7f353697071c"), // Replace with your user's actual Id
            // Add other claims as needed
        };

        var identity = new ClaimsIdentity(claims, "test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(c => c.User).Returns(claimsPrincipal);

        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object,
        };

        _controller = new AuthController(_userManagerMock.Object, _tokenRepoMock.Object, _loggerMock.Object)
        {
            ControllerContext = controllerContext,
        };
    }
    
    [Fact]
    public async Task Register_ReturnsConflict_WhenPhoneNumberExists()
    {
        var request = new RegisterDto
        {
            PhoneNumber = "1234567890",
            FirstName = "John",
            LastName = "Doe",
            Password = "password"
        };

        _userManagerMock.Setup(um => um.FindByNameAsync(request.PhoneNumber))
            .ReturnsAsync(new ApplicationUser());
        
        var result = await _controller.Register(request);
        
        Assert.IsType<ConflictObjectResult>(result);
    }
    
    [Fact]
    public async Task Register_ReturnsOk_WhenUserIsRegistered()
    {
        var request = new RegisterDto
        {
            PhoneNumber = "1234567890",
            FirstName = "John",
            LastName = "Doe",
            Password = "password"
        };

        _userManagerMock.Setup(um => um.FindByNameAsync(request.PhoneNumber))
            .ReturnsAsync((ApplicationUser)null);
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);
        
        var result = await _controller.Register(request);
        
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task Login_ReturnsOk_WhenValidCredentials()
    {
        var request = new LoginDto
        {
            PhoneNumber = "1234567890",
            Password = "password"
        };

        var user = new ApplicationUser { UserName = request.PhoneNumber };
        _userManagerMock.Setup(um => um.FindByNameAsync(request.PhoneNumber))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);

        _tokenRepoMock.Setup(tr => tr.CreateJwtToken(user))
            .Returns("fake_jwt_token");
        
        var result = await _controller.Login(request);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<TokenDto>(okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenInvalidCredentials()
    {
        var request = new LoginDto
        {
            PhoneNumber = "1234567890",
            Password = "wrong_password"
        };

        _userManagerMock.Setup(um => um.FindByNameAsync(request.PhoneNumber))
            .ReturnsAsync((ApplicationUser)null);
        
        var result = await _controller.Login(request);
        
        Assert.IsType<BadRequestObjectResult>(result);
    }

}