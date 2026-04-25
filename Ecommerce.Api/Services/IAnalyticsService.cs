using Ecommerce.Api.Contracts.Analytics;

namespace Ecommerce.Api.Services;

/// <summary>
/// Defines the contract for the AI-driven analytics service.
/// </summary>
public interface IAnalyticsService
{
    Task<AiTrendReportDto> GenerateTrendSummaryReportAsync();
}