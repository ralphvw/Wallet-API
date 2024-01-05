using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WalletWebApi.Models.Domain;
using WalletWebApi.Models.Dto;
using WalletWebApi.Repositories;

namespace WalletWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WalletController: ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public WalletController(UserManager<ApplicationUser> userManager, IWalletRepository walletRepository,
        IMapper mapper)
    {
        _userManager = userManager;
        _walletRepository = walletRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] AddWalletDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var user = await _userManager.GetUserAsync(User);
        bool isDuplicate = await _walletRepository.CheckDuplicateWallet(request.Type, request.AccountNumber, user.Id);
        if(isDuplicate) return Conflict(new { message = "This wallet already exists" });
        bool exceededLimit = await _walletRepository.CheckUserLimit(user.Id, 5);
        if (exceededLimit) return BadRequest("You have exceeded your wallet limit.");
        var wallet = new Wallet
        {
            AccountNumber = request.AccountNumber.Substring(0, 6),
            AccountScheme = request.AccountScheme,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            OwnerId = user.Id,
            Type = request.Type
        };

        wallet = await _walletRepository.CreateWalletAsync(wallet);
        return Ok(_mapper.Map<WalletResponseDto>(wallet));
    }
}