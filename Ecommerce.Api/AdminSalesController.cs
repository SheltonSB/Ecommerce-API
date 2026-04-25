using Ecommerce.Api.Contracts;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/sales")]
[Authorize(Roles = "Admin")]
public class AdminSalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public AdminSalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSales([FromQuery] PagedRequest request, [FromQuery] string? status, [FromQuery] string? customerName)
    {
        var sales = await _saleService.GetAllAsync(request, status, customerName);
        return Ok(sales);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSaleById(int id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        return sale == null ? NotFound() : Ok(sale);
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteSale(int id)
    {
        var success = await _saleService.CompleteAsync(id);
        return success ? Ok() : NotFound();
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelSale(int id)
    {
        var success = await _saleService.CancelAsync(id);
        return success ? Ok() : NotFound();
    }
}