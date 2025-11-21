using System.Security.Claims;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnalyticsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnalyticsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("track")]
    public async Task<IActionResult> TrackEvent([FromBody] InteractionDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var interaction = new UserInteraction
        {
            UserId = userId,
            Action = request.Action,
            ProductId = request.ProductId,
            Metadata = request.Metadata,
            Timestamp = DateTime.UtcNow
        };

        _context.UserInteractions.Add(interaction);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("export")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportData()
    {
        var data = await _context.UserInteractions
            .OrderByDescending(x => x.Timestamp)
            .Take(1000)
            .ToListAsync();

        return Ok(data);
    }
}

public record InteractionDto(string Action, int? ProductId, string? Metadata);
