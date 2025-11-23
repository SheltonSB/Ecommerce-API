using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/ai-agent")]
[Authorize(Roles = "Admin")]
public class AiAgentController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AiAgentController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("trends-report")]
    public async Task<IActionResult> GetTrendsReport()
    {
        var report = await _analyticsService.GenerateTrendSummaryReportAsync();
        return Ok(report);
    }
}