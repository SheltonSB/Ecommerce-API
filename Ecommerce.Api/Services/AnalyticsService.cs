using Ecommerce.Api.Contracts.Analytics;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Ecommerce.Api.Services;

/// <summary>
/// Implements the AI-driven analytics service to track sales and purchase trends.
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _context;

    public AnalyticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AiTrendReportDto> GenerateTrendSummaryReportAsync()
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

        var salesLast30Days = await _context.Sales
            .Where(s => s.SaleDate >= thirtyDaysAgo && s.Status == Domain.SaleStatus.Completed)
            .Include(s => s.SaleItems).ThenInclude(si => si.Product).ThenInclude(p => p.Category)
            .ToListAsync();

        var salesLast7Days = salesLast30Days.Where(s => s.SaleDate >= sevenDaysAgo).ToList();

        var dailySales = salesLast30Days
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new SalesTrendPointDto { Date = g.Key, TotalSales = g.Sum(s => s.FinalAmount) })
            .OrderBy(d => d.Date)
            .ToList();

        var topProducts = salesLast30Days
            .SelectMany(s => s.SaleItems)
            .GroupBy(si => si.Product)
            .Select(g => new ProductPerformanceDto
            {
                ProductId = g.Key.Id,
                ProductName = g.Key.Name,
                TotalQuantitySold = g.Sum(si => si.Quantity),
                TotalRevenue = g.Sum(si => si.TotalPrice)
            })
            .OrderByDescending(p => p.TotalRevenue)
            .Take(5)
            .ToList();

        var categoryPerformance = salesLast30Days
            .SelectMany(s => s.SaleItems)
            .GroupBy(si => si.Product.Category.Name)
            .Select(g => new CategoryPerformanceDto
            {
                CategoryName = g.Key,
                TotalRevenue = g.Sum(si => si.TotalPrice)
            })
            .OrderByDescending(c => c.TotalRevenue)
            .ToList();

        var geoPerformance = salesLast30Days
            .Where(s => !string.IsNullOrWhiteSpace(s.CustomerState) || !string.IsNullOrWhiteSpace(s.CustomerCountry))
            .GroupBy(s => $"{s.CustomerCity}, {s.CustomerState}, {s.CustomerCountry}".Trim(',', ' '))
            .Select(g => new GeospatialPerformanceDto
            {
                Location = g.Key,
                TotalRevenue = g.Sum(s => s.FinalAmount),
                TotalSalesCount = g.Count()
            })
            .OrderByDescending(l => l.TotalRevenue)
            .Take(10)
            .ToList();

        var forecast = GenerateSimpleForecast(salesLast30Days);
        var recommendations = GenerateStrategicRecommendations(topProducts.FirstOrDefault(), geoPerformance.FirstOrDefault());
        var summary = GenerateNaturalLanguageSummary(salesLast7Days, salesLast30Days, topProducts.FirstOrDefault(), forecast);

        return new AiTrendReportDto
        {
            GeneratedAt = DateTime.UtcNow,
            Summary = summary,
            StrategicRecommendations = recommendations,
            DailySalesTrend = dailySales,
            SalesForecast = forecast,
            TopPerformingProducts = topProducts,
            CategoryRevenueDistribution = categoryPerformance,
            GeospatialPerformance = geoPerformance
        };
    }

    private string GenerateNaturalLanguageSummary(List<Domain.Sale> salesLast7Days, List<Domain.Sale> salesLast30Days, ProductPerformanceDto? topProduct, SalesForecastDto forecast)
    {
        var summaryBuilder = new StringBuilder();

        var totalRevenueLast7Days = salesLast7Days.Sum(s => s.FinalAmount);
        var totalRevenueLast30Days = salesLast30Days.Sum(s => s.FinalAmount);

        summaryBuilder.AppendLine($"Analysis for the last 30 days:");
        summaryBuilder.AppendLine($"- Total completed sales revenue: {totalRevenueLast30Days:C}.");
        summaryBuilder.AppendLine($"- Revenue in the last 7 days: {totalRevenueLast7Days:C}.");

        // Trend analysis
        var revenuePrevious7Days = salesLast30Days
            .Where(s => s.SaleDate < DateTime.UtcNow.AddDays(-7) && s.SaleDate >= DateTime.UtcNow.AddDays(-14))
            .Sum(s => s.FinalAmount);

        if (revenuePrevious7Days > 0)
        {
            var weeklyChange = (totalRevenueLast7Days - revenuePrevious7Days) / revenuePrevious7Days;
            if (weeklyChange > 0.05m)
                summaryBuilder.AppendLine($"- Sales are trending up, with a {weeklyChange:P1} increase in revenue this week compared to the previous week.");
            else if (weeklyChange < -0.05m)
                summaryBuilder.AppendLine($"- Sales are trending down, with a {Math.Abs(weeklyChange):P1} decrease in revenue this week compared to the previous week.");
            else
                summaryBuilder.AppendLine("- Sales volume has remained stable over the last two weeks.");
        }

        // Top product highlight
        if (topProduct != null)
        {
            summaryBuilder.AppendLine($"- The top-performing product is '{topProduct.ProductName}', generating {topProduct.TotalRevenue:C} in revenue from {topProduct.TotalQuantitySold} units sold.");
        }

        summaryBuilder.AppendLine($"- Forecast for the next 7 days predicts a revenue of approximately {forecast.PredictedRevenue:C}.");

        return summaryBuilder.ToString();
    }

    private SalesForecastDto GenerateSimpleForecast(List<Domain.Sale> salesLast30Days)
    {
        if (salesLast30Days.Count < 2)
        {
            return new SalesForecastDto { PredictedRevenue = 0 };
        }

        // Simple average daily revenue forecast
        var averageDailyRevenue = salesLast30Days.Sum(s => s.FinalAmount) / 30;
        var predictedRevenue = averageDailyRevenue * 7;

        return new SalesForecastDto
        {
            PredictedRevenue = Math.Max(0, predictedRevenue) // Ensure non-negative forecast
        };
    }

    private StrategicRecommendationsDto GenerateStrategicRecommendations(ProductPerformanceDto? topProduct, GeospatialPerformanceDto? topLocation)
    {
        return new StrategicRecommendationsDto
        {
            Product = topProduct != null ? $"Capitalize on the success of '{topProduct.ProductName}'. Consider bundling it with complementary items or increasing stock." : "Identify and promote potential top-selling products.",
            Price = "Review pricing for underperforming products. Consider offering limited-time discounts to stimulate demand.",
            Place = topLocation != null ? $"'{topLocation.Location}' is a key market, generating {topLocation.TotalRevenue:C}. Target marketing campaigns and logistics for this region." : "Run broad marketing campaigns to identify high-potential geographic markets.",
            Promotion = "Launch a promotional campaign for the top product category. Use email marketing to target repeat customers with special offers."
        };
    }
}