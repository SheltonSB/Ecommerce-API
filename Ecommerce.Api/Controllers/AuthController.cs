using Ecommerce.Api.Contracts;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender)
    {
        _userManager = userManager;
        _configuration = configuration;
        _emailSender = emailSender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var fullName = $"{model.FirstName} {model.LastName}".Trim();
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = fullName,
            PhoneNumber = model.PhoneNumber
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var baseUrl = _configuration["Frontend:BaseUrl"];
        var callbackBase = !string.IsNullOrWhiteSpace(baseUrl)
            ? baseUrl.TrimEnd('/')
            : $"{Request.Scheme}://{Request.Host}";
        var confirmationLink =
            $"{callbackBase}/verify-email?userId={Uri.EscapeDataString(user.Id)}&token={Uri.EscapeDataString(token)}";

        await _emailSender.SendEmailAsync(
            user.Email!,
            "Confirm your email",
            $"<p>Please confirm your email by clicking the link below:</p><p><a href=\"{confirmationLink}\">Confirm Email</a></p>");

        return Ok(new { Message = "User registered successfully. Please check your email to confirm your account." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            if (!user.EmailConfirmed)
            {
                return BadRequest(new { Message = "Email not verified. Please verify your email before logging in." });
            }
            var token = GenerateJwtToken(user);
            return Ok(new AuthResponseDto(token, user.Email!, user.Id));
        }
        return Unauthorized(new { Message = "Invalid login attempt" });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return BadRequest(new { Message = "Invalid user" });

        var result = await _userManager.ConfirmEmailAsync(user, model.Token);
        if (result.Succeeded) return Ok(new { Message = "Email confirmed successfully" });

        return BadRequest(result.Errors);
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}
