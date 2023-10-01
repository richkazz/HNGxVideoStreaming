using HNGxVideoStreaming.Models;

namespace HNGxVideoStreaming.Interface
{
    public interface IUploadService
    {
        public Task<ResponseContext> UploadChunks(string uploadKeya);
        public Task<ResponseContext> UploadComplete(string uploadKey);
        public Task<ResponseContext> StartUpload(string fileName);
        public Task<ResponseContext> StreamVideo(string uploadKey);
        public Task<ResponseContext> GetAll();
    }
}
