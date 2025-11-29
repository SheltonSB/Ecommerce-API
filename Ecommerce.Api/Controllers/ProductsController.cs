using Ecommerce.Api.Contracts;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

/// <summary>
/// API controller for managing products
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[AllowAnonymous]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all products with pagination and filtering
    /// </summary>
    /// <param name="request">Pagination parameters</param>
    /// <param name="categoryId">Optional category filter</param>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="isActive">Optional active status filter</param>
    /// <returns>Paginated list of products</returns>
    /// <response code="200">Returns the paginated list of products</response>
    [HttpGet]
    [ProducesResponseType(typeof(Paged<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Paged<ProductListItemDto>>> GetAll(
        [FromQuery] PagedRequest request,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _productService.GetAllAsync(request, categoryId, searchTerm, isActive);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    /// <response code="200">Returns the product</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { message = $"Product with ID {id} not found" });
        }
        return Ok(result);
    }

    /// <summary>
    /// Gets a product by SKU
    /// </summary>
    /// <param name="sku">Product SKU</param>
    /// <returns>Product details</returns>
    /// <response code="200">Returns the product</response>
    /// <response code="404">Product not found</response>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetBySku(string sku)
    {
        var result = await _productService.GetBySkuAsync(sku);
        if (result == null)
        {
            return NotFound(new { message = $"Product with SKU '{sku}' not found" });
        }
        return Ok(result);
    }

    /// <summary>
    /// Gets products with low stock
    /// </summary>
    /// <param name="threshold">Stock threshold (default 10)</param>
    /// <returns>List of low stock products</returns>
    /// <response code="200">Returns low stock products</response>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IEnumerable<ProductListItemDto>), StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ProductListItemDto>>> GetLowStock([FromQuery] int threshold = 10)
    {
        var result = await _productService.GetLowStockProductsAsync(threshold);
        return Ok(result);
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>List of products in the category</returns>
    /// <response code="200">Returns products in the category</response>
    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(typeof(IEnumerable<ProductListItemDto>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductListItemDto>>> GetByCategory(int categoryId)
    {
        var result = await _productService.GetByCategoryAsync(categoryId);
        return Ok(result);
    }

    /// <summary>
    /// Gets price history for a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Price history entries</returns>
    /// <response code="200">Returns price history</response>
    [HttpGet("{id}/price-history")]
    [ProducesResponseType(typeof(IEnumerable<PriceHistoryDto>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PriceHistoryDto>>> GetPriceHistory(int id)
    {
        var result = await _productService.GetPriceHistoryAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="dto">Product creation data</param>
    /// <returns>Created product</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid data provided</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="dto">Product update data</param>
    /// <returns>Updated product</returns>
    /// <response code="200">Product updated successfully</response>
    /// <response code="400">Invalid data provided</response>
    /// <response code="404">Product not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _productService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Updates product stock quantity
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="dto">Stock update data</param>
    /// <returns>Updated product</returns>
    /// <response code="200">Stock updated successfully</response>
    /// <response code="400">Invalid data provided</response>
    /// <response code="404">Product not found</response>
    [HttpPatch("{id}/stock")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> UpdateStock(int id, [FromBody] UpdateStockDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _productService.UpdateStockAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Soft deletes a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Product deleted successfully</response>
    /// <response code="404">Product not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Product with ID {id} not found" });
        }
        return NoContent();
    }

    /// <summary>
    /// Restores a soft-deleted product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Product restored successfully</response>
    /// <response code="404">Product not found</response>
    [HttpPost("{id}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Restore(int id)
    {
        var result = await _productService.RestoreAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Deleted product with ID {id} not found" });
        }
        return NoContent();
    }
}
