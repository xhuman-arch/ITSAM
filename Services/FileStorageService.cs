namespace ITServiceAssetManagement.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        // Ekstensi yang diizinkan — batasi supaya tidak sembarang file bisa diupload
        private static readonly string[] AllowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx"
        };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<(string fileName, string filePath)> SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file.Length == 0)
                throw new InvalidOperationException("File kosong.");

            if (file.Length > MaxFileSizeBytes)
                throw new InvalidOperationException("Ukuran file maksimal 5 MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("Tipe file tidak diizinkan. Gunakan jpg, png, gif, webp, pdf, doc, atau docx.");

            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            Directory.CreateDirectory(uploadsRoot);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadsRoot, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/{subFolder}/{uniqueFileName}";
            return (file.FileName, relativePath);
        }

        public void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
