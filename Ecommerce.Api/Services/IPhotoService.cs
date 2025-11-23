using CloudinaryDotNet.Actions;

namespace Ecommerce.Api.Services;

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
}