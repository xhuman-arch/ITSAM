namespace ITServiceAssetManagement.Services
{
    public interface ISlaService
    {
        DateTime CalculateDueDate(string priority, DateTime createdAt);
    }
}
