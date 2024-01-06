using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WalletWebApi.Controllers;
using WalletWebApi.Models.Domain;
using WalletWebApi.Models.Dto;
using WalletWebApi.Repositories;

namespace WalletApi.Tests.Controllers;

public class WalletControllerTests
{
    private readonly Mock<IUserStore<ApplicationUser>> _storeMock = new();
    
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IWalletRepository> _walletRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<WalletController>> _loggerMock = new();

    private readonly WalletController _controller;

    public WalletControllerTests()
    
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

        _controller = new WalletController(_userManagerMock.Object, _walletRepoMock.Object, _mapperMock.Object, _loggerMock.Object)
        {
            ControllerContext = controllerContext,
        };
    }

    [Fact]
    public async Task CreateWallet_ReturnsConflict_WhenWalletExists()
    {
        // Arrange
        var request = new AddWalletDto()
        {
            Type = "momo",
            AccountNumber = "1234567890"
        };
        var userId = "13a975c3-d157-4443-b900-7f353697071c";

        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(new ApplicationUser { Id = userId });

        _walletRepoMock.Setup(repo => repo.CheckDuplicateWallet(request.Type, request.AccountNumber, userId))
            .ReturnsAsync(true);
        
        var result = await _controller.CreateWallet(request);
        
        Assert.IsType<ConflictObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateWallet_ReturnsOk_WhenWalletCreatedSuccessfully()
    {
        var request = new AddWalletDto()
        {
            Type = "momo",
            AccountNumber = "1234567890",
            Name = "Test Wallet",
            AccountScheme = "visa",
        };

        var userId = "13a975c3-d157-4443-b900-7f353697071c";
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(new ApplicationUser { Id = userId });

        _walletRepoMock.Setup(repo => repo.CheckDuplicateWallet(request.Type, request.AccountNumber, userId))
            .ReturnsAsync(false);

        _walletRepoMock.Setup(repo => repo.CheckUserLimit(userId, 5))
            .ReturnsAsync(false); 

        _walletRepoMock.Setup(repo => repo.CreateWalletAsync(It.IsAny<Wallet>()))
            .ReturnsAsync(new Wallet());
        
        var result = await _controller.CreateWallet(request);
        
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateWallet_ReturnsBadRequest_WhenExceedingWalletLimit()
    {
        var request = new AddWalletDto()
        {
            Type = "momo",
            AccountNumber = "1234567890",
            Name = "Test Wallet",
            AccountScheme = "visa",
        };

        var userId = "13a975c3-d157-4443-b900-7f353697071c";
        
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(new ApplicationUser { Id = userId });

        _walletRepoMock.Setup(repo => repo.CheckUserLimit(userId, 5))
            .ReturnsAsync(true);
        
        var result = await _controller.CreateWallet(new AddWalletDto());
        
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task DeleteWallet_ReturnsOk_WhenDeletingOwnedWallet()
    {
        var walletId = 1;
        var userId = "13a975c3-d157-4443-b900-7f353697071c";

        var user = new ApplicationUser { Id = userId };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var walletToDelete = new Wallet { ID = walletId, OwnerId = userId };
        _walletRepoMock.Setup(repo => repo.GetWalletAsync(walletId))
            .ReturnsAsync(walletToDelete);

        _walletRepoMock.Setup(repo => repo.IsWalletOwner(walletToDelete, userId))
            .Returns(true);
        
        var result = await _controller.DeleteWallet(walletId);
        
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task DeleteWallet_ReturnsNotFound_WhenWalletNotExists()
    {
        var walletId = 123;
        var userId = "13a975c3-d157-4443-b900-7f353697071c";

        var user = new ApplicationUser { Id = userId };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(user);

        _walletRepoMock.Setup(repo => repo.GetWalletAsync(walletId))
            .ReturnsAsync((Wallet)null);
        
        var result = await _controller.DeleteWallet(walletId);
        
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task GetWallet_ReturnsOk_WhenFetchingOwnedWallet()
    {
        var walletId = 123;
        var userId = "13a975c3-d157-4443-b900-7f353697071c";

        var user = new ApplicationUser { Id = userId };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var walletToFetch = new Wallet { ID = walletId, OwnerId = userId };
        _walletRepoMock.Setup(repo => repo.GetWalletAsync(walletId))
            .ReturnsAsync(walletToFetch);

        _walletRepoMock.Setup(repo => repo.IsWalletOwner(walletToFetch, userId))
            .Returns(true);
        
        var result = await _controller.GetWallet(walletId);
        
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task GetWallet_ReturnsNotFound_WhenWalletNotExists()
    {
        var walletId = 123;
        var userId = "13a975c3-d157-4443-b900-7f353697071c";

        var user = new ApplicationUser { Id = userId };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(user);

        _walletRepoMock.Setup(repo => repo.GetWalletAsync(walletId))
            .ReturnsAsync((Wallet)null);
        
        var result = await _controller.GetWallet(walletId);
        
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task GetUserWallets_ReturnsOk_WhenWalletsExist()
    {
        var userId = "13a975c3-d157-4443-b900-7f353697071c";

        var user = new ApplicationUser { Id = userId };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var userWallets = new List<Wallet>
        {
            new Wallet { ID = 1, OwnerId = userId, Name = "Wallet"},
            new Wallet { ID = 2, OwnerId = userId, Name = "Wallet"}
        };
    
        var walletResponseDtos = userWallets.Select(wallet => new WalletResponseDto
        {
            Name = wallet.Name,
        }).ToList();

        _walletRepoMock.Setup(repo => repo.GetAllWalletsAsync(userId))
            .ReturnsAsync(userWallets);

        _mapperMock.Setup(mapper => mapper.Map<List<WalletResponseDto>>(It.IsAny<List<Wallet>>()))
            .Returns(walletResponseDtos);
        
        var result = await _controller.GetUserWallets();
        
        Assert.NotNull(result);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var wallets = okResult.Value as List<WalletResponseDto>;
        Assert.NotNull(wallets);
        Assert.Equal(walletResponseDtos.Count, wallets.Count);
    }
    
}