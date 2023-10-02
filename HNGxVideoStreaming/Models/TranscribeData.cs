using System.Text.Json.Serialization;
using TestmeQA.Domain.Common;

namespace HNGxVideoStreaming.Models
{
    public class TranscribeData : EntityBase
    {
        public string Text { get; set; } = string.Empty;
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int UploadKeyId { get; set; }
        // Add a foreign key constraint to the UploadKeyId property
        [JsonIgnore]
        public virtual UploadContext UploadContext { get; set; }
    }
}
