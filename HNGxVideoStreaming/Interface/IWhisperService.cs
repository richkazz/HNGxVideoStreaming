using HNGxVideoStreaming.Models;

namespace HNGxVideoStreaming.Interface
{
    public interface IWhisperService
    {
        public Task<List<TranscribeData>> Transcribe(string path, int uploadKeyId);
    }
}
