using Microsoft.AspNetCore.Hosting;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Infrastructure.Services;

public class LocalStorageService : IStorageService
{
    private readonly string _uploadsRoot;

    public LocalStorageService(IWebHostEnvironment env)
    {
        _uploadsRoot = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        Directory.CreateDirectory(_uploadsRoot);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string subPath, CancellationToken cancellationToken = default)
    {
        var dir = Path.Combine(_uploadsRoot, subPath);
        Directory.CreateDirectory(dir);

        var ext = Path.GetExtension(fileName);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(dir, storedName);

        using var output = File.Create(fullPath);
        await fileStream.CopyToAsync(output, cancellationToken);

        return $"/uploads/{subPath}/{storedName}".Replace("\\", "/");
    }

    public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileUrl)) return Task.CompletedTask;

        var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (File.Exists(fullPath)) File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
