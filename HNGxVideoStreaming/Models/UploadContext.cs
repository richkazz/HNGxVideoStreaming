using System.ComponentModel.DataAnnotations;
using TestmeQA.Domain.Common;

namespace HNGxVideoStreaming.Models
{
    public class UploadContext : EntityBase
    {
        public string UploadKey { get; set; }
        public string FileName { get; set; }
        public int currentId { get; set; }
        public bool isUploading { get; set; } = false;
        // Add a property to store a collection of TranscribeData records
        public virtual ICollection<TranscribeData> TranscribedData { get; set; }
    }
}
