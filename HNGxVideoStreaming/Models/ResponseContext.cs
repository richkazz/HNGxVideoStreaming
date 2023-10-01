namespace HNGxVideoStreaming.Models
{
    public class ResponseContext
    {
        public dynamic Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string ErrorMessage { get; set; }
    }
    public class ResponseWithStreamContext
    {
        public UploadContext uploadContext { get; set; } = null!;
        public FileStream stream { get; set; } = null!;
    }
    public class ResponseUploadContext
    {
        public UploadContext? uploadContext { get; set; }
        public string? VideoLink { get; set; }
    }

}
