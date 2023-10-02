using HNGxVideoStreaming.Data;
using HNGxVideoStreaming.Models;

namespace HNGxVideoStreaming.BackgroundServices
{
    public class CleanupBackgroundService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public CleanupBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Set up a timer to run the cleanup task every 30 minutes
            _timer = new Timer(DoCleanup, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoCleanup(object state)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<HNGxVideoStreamingDbContext>();

                    // Get all UploadContexts where isUploading is true and LastModifiedDate is greater than 10 minutes ago
                    var thresholdTime = DateTime.UtcNow.AddMinutes(-30);
                    var uploadContextsToDelete = dbContext.UploadContexts
                        .Where(u => u.isUploading && u.LastModifiedDate < thresholdTime)
                        .ToList();

                    foreach (var uploadContext in uploadContextsToDelete)
                    {
                        // Delete associated temporary chunk files on disk
                        DeleteTemporaryChunkFiles(uploadContext);

                        // Remove the upload context from the database
                        dbContext.UploadContexts.Remove(uploadContext);
                    }

                    // Save changes to the database
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
            
        }

        private void DeleteTemporaryChunkFiles(UploadContext uploadContext)
        {
            try
            {
                var fileName = uploadContext.UploadKey;
                string tempPath = Path.GetTempPath();
                string newPath = Path.Combine(tempPath, fileName);
                string[] filePaths = Directory.GetFiles(tempPath).Where(p => p.Contains(fileName)).OrderBy(p => Int32.Parse(p.Replace(fileName, "$").Split('$')[1])).ToArray();
                foreach (string filePath in filePaths)
                {
                    #region Delete chunk file from disk if fails
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {

            }
           
        }
    }
}
