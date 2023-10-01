using System.Text.Json.Serialization;

namespace HNGxVideoStreaming.Models
{
    public class TranscribeData
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int UploadKeyId { get; set; }
        // Add a foreign key constraint to the UploadKeyId property
        [JsonIgnore]
        public virtual UploadContext UploadContext { get; set; }
    }
}
