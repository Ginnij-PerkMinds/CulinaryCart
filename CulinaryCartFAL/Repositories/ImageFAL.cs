using CulinaryCart.CulinaryFAL;

public class ImageFAL : IImageFAL
{
    private readonly IWebHostEnvironment _env;

    public ImageFAL(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string SaveImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("Invalid image file");

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var path = Path.Combine(_env.WebRootPath, "uploads","images", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return "/uploads/images/" + fileName; // ✅ relative path for DB
    }

    public void DeleteImage(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return;

        var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

    }
}
