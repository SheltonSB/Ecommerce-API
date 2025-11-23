using Ecommerce.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users
            .Select(u => new { u.Id, u.UserName, u.Email, u.EmailConfirmed })
            .ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { user.Id, user.UserName, user.Email, user.EmailConfirmed, Roles = roles });
    }

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> UpdateUserRoles(string id, [FromBody] List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!result.Succeeded) return BadRequest(result.Errors);

        result = await _userManager.AddToRolesAsync(user, roles);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok(await _userManager.GetRolesAsync(user));
    }
}