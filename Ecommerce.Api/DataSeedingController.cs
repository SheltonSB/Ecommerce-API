using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Ecommerce.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/data-seeding")]
[Authorize(Roles = "Admin")]
public class DataSeedingController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<DataSeedingController> _logger;

    public DataSeedingController(AppDbContext context, ILogger<DataSeedingController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("seed-sales-from-csv")]
    public async Task<IActionResult> SeedSalesData(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        _logger.LogInformation("Starting to seed sales data from uploaded CSV file.");

        var salesToCreate = new List<Sale>();
        using var reader = new StreamReader(file.OpenReadStream());
        await reader.ReadLineAsync(); // Skip header row

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var values = line.Split(',');

            try
            {
                // This is a simplified parser. A real implementation would be more robust.
                // Assumes CSV format: SaleDate,CustomerName,CustomerCity,CustomerState,CustomerCountry,ProductId,Quantity,UnitPrice
                var sale = new Sale
                {
                    SaleDate = DateTime.Parse(values[0], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime(),
                    CustomerName = values[1],
                    CustomerCity = values[2],
                    CustomerState = values[3],
                    CustomerCountry = values[4],
                    Status = SaleStatus.Completed,
                    SaleNumber = $"SEED-{Guid.NewGuid().ToString().Substring(0, 8)}"
                };

                var product = await _context.Products.FindAsync(int.Parse(values[5]));
                if (product != null)
                {
                    sale.AddSaleItem(product, int.Parse(values[6]));
                    salesToCreate.Add(sale);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Skipping invalid row in CSV: {Row}. Error: {Error}", line, ex.Message);
            }
        }

        await _context.Sales.AddRangeAsync(salesToCreate);
        var recordsSaved = await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully seeded {Count} new sales records.", recordsSaved);
        return Ok(new { Message = $"Successfully seeded {recordsSaved} new sales records." });
    }
}