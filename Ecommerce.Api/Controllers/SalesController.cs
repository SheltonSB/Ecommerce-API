using Ecommerce.Api.Contracts;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

/// <summary>
/// API controller for managing sales transactions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;
    private readonly ILogger<SalesController> _logger;

    public SalesController(ISaleService saleService, ILogger<SalesController> logger)
    {
        _saleService = saleService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all sales with pagination and filtering
    /// </summary>
    /// <param name="request">Pagination parameters</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="customerName">Optional customer name filter</param>
    /// <returns>Paginated list of sales</returns>
    /// <response code="200">Returns the paginated list of sales</response>
    [HttpGet]
    [ProducesResponseType(typeof(Paged<SaleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Paged<SaleListItemDto>>> GetAll(
        [FromQuery] PagedRequest request,
        [FromQuery] string? status = null,
        [FromQuery] string? customerName = null)
    {
        var result = await _saleService.GetAllAsync(request, status, customerName);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific sale by ID
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>Sale details</returns>
    /// <response code="200">Returns the sale</response>
    /// <response code="404">Sale not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaleDto>> GetById(int id)
    {
        var result = await _saleService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { message = $"Sale with ID {id} not found" });
        }
        return Ok(result);
    }

    /// <summary>
    /// Gets sales within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of sales in the date range</returns>
    /// <response code="200">Returns sales in the date range</response>
    [HttpGet("date-range")]
    [ProducesResponseType(typeof(IEnumerable<SaleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SaleListItemDto>>> GetByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await _saleService.GetByDateRangeAsync(startDate, endDate);
        return Ok(result);
    }

    /// <summary>
    /// Gets sales summary statistics
    /// </summary>
    /// <returns>Sales summary data</returns>
    /// <response code="200">Returns sales summary</response>
    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetSummary()
    {
        var result = await _saleService.GetSalesSummaryAsync();
        return Ok(result);
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    /// <param name="dto">Sale creation data</param>
    /// <returns>Created sale</returns>
    /// <response code="201">Sale created successfully</response>
    /// <response code="400">Invalid data provided</response>
    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaleDto>> Create([FromBody] CreateSaleDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _saleService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <param name="dto">Sale update data</param>
    /// <returns>Updated sale</returns>
    /// <response code="200">Sale updated successfully</response>
    /// <response code="400">Invalid data provided</response>
    /// <response code="404">Sale not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaleDto>> Update(int id, [FromBody] UpdateSaleDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _saleService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Completes a pending sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Sale completed successfully</response>
    /// <response code="404">Sale not found</response>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(int id)
    {
        var result = await _saleService.CompleteAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Sale with ID {id} not found" });
        }
        return NoContent();
    }

    /// <summary>
    /// Cancels a sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Sale cancelled successfully</response>
    /// <response code="404">Sale not found</response>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _saleService.CancelAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Sale with ID {id} not found" });
        }
        return NoContent();
    }

    /// <summary>
    /// Adds payment information to a sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <param name="dto">Payment information</param>
    /// <returns>No content</returns>
    /// <response code="204">Payment added successfully</response>
    /// <response code="400">Invalid data provided</response>
    /// <response code="404">Sale not found</response>
    [HttpPost("{id}/payment")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPayment(int id, [FromBody] CreatePaymentInfoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _saleService.AddPaymentAsync(id, dto);
        if (!result)
        {
            return NotFound(new { message = $"Sale with ID {id} not found" });
        }
        return NoContent();
    }
}

