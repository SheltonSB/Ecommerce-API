using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Api.Services;

/// <summary>
/// Uploads images to Cloudinary and returns secure URLs.
/// </summary>
public class ImageService
{
    private readonly Cloudinary? _cloudinary;
    private readonly ILogger<ImageService> _logger;

    public ImageService(IConfiguration config, ILogger<ImageService> logger)
    {
        _logger = logger;
        var cloud = config["Cloudinary:CloudName"];
        var apiKey = config["Cloudinary:ApiKey"];
        var apiSecret = config["Cloudinary:ApiSecret"];

        if (!string.IsNullOrWhiteSpace(cloud) && !string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(apiSecret))
        {
            _cloudinary = new Cloudinary(new Account(cloud, apiKey, apiSecret));
        }
        else
        {
            _logger.LogWarning("Cloudinary settings missing; image uploads will be skipped.");
        }
    }

    public async Task<string?> UploadAsync(IFormFile file)
    {
        if (_cloudinary == null)
        {
            return null;
        }

        if (file.Length == 0)
        {
            return null;
        }

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(800).Height(800).Crop("fill").Quality("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return result.SecureUrl.ToString();
        }

        _logger.LogError("Cloudinary upload failed with status {StatusCode}: {Error}", result.StatusCode, result.Error?.Message);
        return null;
    }
}
