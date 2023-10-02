using HNGxVideoStreaming.Interface;
using HNGxVideoStreaming.Models;
using Microsoft.AspNetCore.Mvc;

namespace HNGxVideoStreaming.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class VideoUploadController : ControllerBase
    {
        private readonly ILogger<VideoUploadController> _logger;
        private readonly IUploadService UploadService;
        private readonly IWhisperService WhisperService;

        public VideoUploadController(
             IWhisperService whisperService,
             ILogger<VideoUploadController> logger,
            IUploadService uploadService)
        {
            _logger = logger;
            WhisperService = whisperService;
            UploadService = uploadService;
        }

        [HttpPost("UploadChunks")]
        public async Task<IActionResult> UploadChunks(string uploadKey)
        {
            return Ok(await UploadService.UploadChunks(uploadKey));
        }

        [HttpPost("startUpload")]
        public async Task<IActionResult> StartUpload(string fileName)
        {
            return Ok(await UploadService.StartUpload(fileName));
        }
        [HttpPost("UploadComplete")]
        public async Task<IActionResult> UploadComplete(string uploadKey)
        {
            return Ok(await UploadService.UploadComplete(uploadKey));
        }
        [HttpDelete("DeleteVideo")]
        public async Task<IActionResult> DeleteVideo(string uploadKey)
        {
            return Ok(await UploadService.DeleteVideo(uploadKey));
        }
        [HttpGet("StreamVideo/{uploadKey}")]
        public async Task<IActionResult> StreamVideo(string uploadKey)
        {
            try
            {
                var result = await UploadService.StreamVideo(uploadKey);
                if (!result.IsSuccess)
                {
                    return Ok(result);
                }
                var ddata = (ResponseWithStreamContext)result.Data;
                var stream = ddata.stream;

                // Set response headers for streaming
                Response.Headers.Add("Content-Disposition", new[] { "inline; filename=" + ddata.uploadContext.FileName });
                Response.Headers.Add("Content-Type", new[] { "video/mp4" });

                return File(stream, "video/mp4");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error streaming video.");
                return StatusCode(500, "An error occurred while streaming the video.");
            }
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await UploadService.GetAll());
        }
    }
}