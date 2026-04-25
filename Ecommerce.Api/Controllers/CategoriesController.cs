using Ecommerce.Api.Contracts;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

/// <summary>
/// API controller for managing product categories
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all categories with pagination
    /// </summary>
    /// <param name="request">Pagination parameters</param>
    /// <returns>Paginated list of categories</returns>
    /// <response code="200">Returns the paginated list of categories</response>
    [HttpGet]
    [ProducesResponseType(typeof(Paged<CategoryListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Paged<CategoryListItemDto>>> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _categoryService.GetAllAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Gets all categories without pagination
    /// </summary>
    /// <returns>List of all categories</returns>
    /// <response code="200">Returns all categories</response>
    [HttpGet("simple")]
    [ProducesResponseType(typeof(IEnumerable<CategoryListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryListItemDto>>> GetAllSimple()
    {
        var result = await _categoryService.GetAllSimpleAsync();
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Category details</returns>
    /// <response code="200">Returns the category</response>
    /// <response code="404">Category not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found" });
        }
        return Ok(result);
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="dto">Category creation data</param>
    /// <returns>Created category</returns>
    /// <response code="201">Category created successfully</response>
    /// <response code="400">Invalid data provided</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="dto">Category update data</param>
    /// <returns>Updated category</returns>
    /// <response code="200">Category updated successfully</response>
    /// <response code="400">Invalid data provided</response>
    /// <response code="404">Category not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _categoryService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Soft deletes a category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Category deleted successfully</response>
    /// <response code="404">Category not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Category with ID {id} not found" });
        }
        return NoContent();
    }

    /// <summary>
    /// Restores a soft-deleted category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Category restored successfully</response>
    /// <response code="404">Category not found</response>
    [HttpPost("{id}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Restore(int id)
    {
        var result = await _categoryService.RestoreAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Deleted category with ID {id} not found" });
        }
        return NoContent();
    }
}
