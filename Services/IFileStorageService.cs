namespace ITServiceAssetManagement.Services
{
    public interface IFileStorageService
    {
        Task<(string fileName, string filePath)> SaveFileAsync(IFormFile file, string subFolder);
        void DeleteFile(string filePath);
    }
}
