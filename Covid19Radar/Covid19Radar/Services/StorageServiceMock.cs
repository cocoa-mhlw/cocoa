using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class StorageServiceMock : IStorageService
    {
        public async Task<bool> UploadAsync(string endpoint, string uploadPath, string accountName, string sasToken, string sourceFilePath)
        {
            await Task.Delay(500);
            return true;
        }
    }
}
