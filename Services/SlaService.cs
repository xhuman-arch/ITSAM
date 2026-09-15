namespace ITServiceAssetManagement.Services
{
    // Aturan SLA sederhana: makin tinggi prioritas, makin cepat batas waktunya.
    // Bisa disesuaikan sesuai kebijakan perusahaan.
    public class SlaService : ISlaService
    {
        private static readonly Dictionary<string, int> SlaHours = new()
        {
            { "HIGH", 4 },     // 4 jam
            { "MEDIUM", 24 },  // 1 hari
            { "LOW", 72 }      // 3 hari
        };

        public DateTime CalculateDueDate(string priority, DateTime createdAt)
        {
            var hours = SlaHours.TryGetValue(priority, out var h) ? h : 24;
            return createdAt.AddHours(hours);
        }
    }
}
