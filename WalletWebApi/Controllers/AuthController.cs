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

    public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository)
    {
        _userManager = userManager;
        _tokenRepository = tokenRepository;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var newUser = new ApplicationUser
        {
            UserName = request.PhoneNumber,
            Email = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        
        if(!result.Succeeded) return BadRequest("Something went wrong");
        
        return Ok("User was registered. Please login");
    }
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.PhoneNumber);

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