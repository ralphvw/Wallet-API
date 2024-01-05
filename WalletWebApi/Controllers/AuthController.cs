using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WalletWebApi.Models.Domain;
using WalletWebApi.Models.Dto;
using WalletWebApi.Repositories;

namespace WalletWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController: ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenRepository _tokenRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _tokenRepository = tokenRepository;
        _logger = logger;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var user = await _userManager.FindByNameAsync(request.PhoneNumber);
        if (user != null) return Conflict(new { message = "Phone number already exists" });
        
        var newUser = new ApplicationUser
        {
            UserName = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();

            // You can log the errors for debugging purposes
            foreach (var error in errors)
            {
                _logger.LogError($"User creation error: {error}");
            }
            
            return BadRequest("Something went wrong");
        }
        
        return Ok("User was registered. Please login");
    }
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _userManager.FindByNameAsync(request.PhoneNumber);

        if (user != null)
        {
            var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (checkPassword)
            {
                var jwtToken = _tokenRepository.CreateJwtToken(user);
                var response = new TokenDto()
                    {
                        JwtToken = jwtToken
                    };
                return Ok(response);
                
            }
        }

        return BadRequest("Invalid login");
    }
}