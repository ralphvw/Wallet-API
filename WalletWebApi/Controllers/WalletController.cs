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
    private readonly ILogger<WalletController> _logger;

    public WalletController(UserManager<ApplicationUser> userManager, IWalletRepository walletRepository,
        IMapper mapper, ILogger<WalletController> logger)
    {
        _userManager = userManager;
        _walletRepository = walletRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] AddWalletDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var isDuplicate = await _walletRepository.CheckDuplicateWallet(request.Type, request.AccountNumber, user.Id);
        if (isDuplicate) return Conflict(new { message = "This wallet already exists" });
        var exceededLimit = await _walletRepository.CheckUserLimit(user.Id, 5);
        if (exceededLimit) return BadRequest("You have exceeded your wallet limit.");
        var wallet = new Wallet
        {
            AccountNumber = request.AccountNumber.Substring(0, 6),
            AccountScheme = request.AccountScheme.ToLower(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            OwnerId = user.Id,
            Type = request.Type.ToLower()
        };

        wallet = await _walletRepository.CreateWalletAsync(wallet);
        return Ok(_mapper.Map<WalletResponseDto>(wallet));
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> DeleteWallet([FromRoute] int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var wallet = await _walletRepository.GetWalletAsync(id);
        if (wallet == null) return NotFound();
        if (!_walletRepository.IsWalletOwner(wallet, user.Id)) return Forbid();
        await _walletRepository.DeleteWalletAsync(wallet);
        return Ok(_mapper.Map<WalletResponseDto>(wallet));
    }
    
    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetWallet([FromRoute] int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var wallet = await _walletRepository.GetWalletAsync(id);
        if (wallet == null) return NotFound();
        if (!_walletRepository.IsWalletOwner(wallet, user.Id)) return Forbid();
        return Ok(_mapper.Map<WalletResponseDto>(wallet));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserWallets()
    {
        var user = await _userManager.GetUserAsync(User);
        var wallets = await _walletRepository.GetAllWalletsAsync(user.Id);
        return Ok(_mapper.Map<List<WalletResponseDto>>(wallets));
    }
    
}