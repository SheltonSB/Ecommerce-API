using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/photos")]
[Authorize(Roles = "Admin")]
public class AdminPhotosController : ControllerBase
{
    private readonly IPhotoService _photoService;

    public AdminPhotosController(IPhotoService photoService)
    {
        _photoService = photoService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadPhoto(IFormFile file)
    {
        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null)
        {
            return BadRequest(result.Error.Message);
        }

        return Ok(new { url = result.SecureUrl.AbsoluteUri });
    }
}