using HNGxVideoStreaming.Controllers;
using HNGxVideoStreaming.Data;
using HNGxVideoStreaming.Helpers;
using HNGxVideoStreaming.Interface;
using HNGxVideoStreaming.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HNGxVideoStreaming.Services
{
    public class UploadService : IUploadService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IConfiguration configuration;
        private IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<UploadService> _logger;
        private readonly ResponseContext _responseData;
        private readonly IWhisperService WhisperService;
        private readonly HNGxVideoStreamingDbContext _dbContext;
        public int chunkSize;
        private string tempFolder;
        private string tempAudioFolder;
        public UploadService(
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor,
            IWhisperService whisperService,
            ILogger<UploadService> logger, HNGxVideoStreamingDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            WhisperService = whisperService;
            _hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
            chunkSize = 1048576 * Convert.ToInt32(configuration["ChunkSize"]);
            tempFolder = Path.Combine(hostingEnvironment.ContentRootPath, "videos");
            tempAudioFolder = Path.Combine(hostingEnvironment.ContentRootPath, "audios");
            _responseData = new ResponseContext();
        }
        public async Task<ResponseContext> StartUpload(string fileName)
        {
            try
            {
                _logger.LogInformation("Starting upload of file {fileName}", fileName);
                string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // You can customize this as needed
                int keyLength = 16;

                string generatedKey = GenerateRandomKey(allowedChars, keyLength) + ".mp4";
                _dbContext.UploadContexts.Add(new UploadContext() { UploadKey = generatedKey, currentId = 1, FileName = fileName, isUploading = true }); ;
                await _dbContext.SaveChangesAsync();
                _responseData.Data = new { uploadKey = generatedKey };
                _logger.LogInformation("StartUpload: Successfully started upload for file: {FileName}", fileName);
            }
            catch (Exception ex)
            {
                _responseData.ErrorMessage = ex.Message;
                _responseData.IsSuccess = false;
                _logger.LogError(ex, "StartUpload: Error starting upload for file: {FileName}", fileName);
            }
            return _responseData;
        }

        public async Task<ResponseContext> UploadChunks(string uploadKey)
        {
            try
            {
                #region check if upload key exists
                var uploadContext = await _dbContext.UploadContexts.FirstOrDefaultAsync(x => x.UploadKey == uploadKey);
                if (uploadContext is null)
                {
                    _responseData.ErrorMessage = "Upload key dose not exist";
                    _responseData.IsSuccess = false;
                    return _responseData;
                }
                #endregion
                #region check if upload has ended
                if (!uploadContext.isUploading)
                {
                    _responseData.ErrorMessage = "Upload as been closed";
                    _responseData.IsSuccess = false;
                    return _responseData;
                }
                #endregion
                var fileName = uploadKey;
                var chunkNumber = uploadContext.currentId.ToString();
                string newpath = Path.Combine(tempFolder, fileName + chunkNumber);
                using (FileStream fs = File.Create(newpath))
                {
                    byte[] bytes = new byte[chunkSize];
                    int bytesRead = 0;
                    while ((bytesRead = await _httpContextAccessor.HttpContext.Request.Body.ReadAsync(bytes, 0, bytes.Length)) > 0)
                    {
                        fs.Write(bytes, 0, bytesRead);
                    }
                }
                uploadContext.currentId++;
                _dbContext.UploadContexts.Update(uploadContext);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("UploadChunks: Successfully uploaded chunk for uploadKey: {UploadKey}", uploadKey);

            }
            catch (Exception ex)
            {
                _responseData.ErrorMessage = ex.Message;
                _responseData.IsSuccess = false;
                _logger.LogError(ex, "UploadChunks: Error uploading chunk for uploadKey: {UploadKey}", uploadKey);
            }
            return _responseData;
        }

        public async Task<ResponseContext> UploadComplete(string uploadKey)
        {
            try
            {
                #region check if upload key exists
                var uploadContext = await _dbContext.UploadContexts.FirstOrDefaultAsync(x => x.UploadKey == uploadKey);
                if (uploadContext is null)
                {
                    _responseData.ErrorMessage = "Upload key dose not exist";
                    _responseData.IsSuccess = false;
                    return _responseData;
                }
                #endregion
                #region merge video chunks into one
                var fileName = uploadKey;
                string tempPath = tempFolder;
                string newPath = Path.Combine(tempPath, fileName);
                string[] filePaths = Directory.GetFiles(tempPath).Where(p => p.Contains(fileName)).OrderBy(p => Int32.Parse(p.Replace(fileName, "$").Split('$')[1])).ToArray();
                foreach (string filePath in filePaths)
                {
                    MergeChunks(newPath, filePath);
                }
                File.Move(Path.Combine(tempPath, fileName), Path.Combine(tempFolder, fileName));
                #endregion


                string videoFilePath = Path.Combine(tempFolder, fileName);
                string audioFilePath = Path.Combine(tempAudioFolder, AudioHelpers.ChangeExtensionToMp3(uploadKey));
                AudioHelpers.ExtractAudioFromVideo(videoFilePath, audioFilePath);

                uploadContext.isUploading = false;
                _dbContext.UploadContexts.Update(uploadContext);

                var list = await WhisperService.Transcribe(audioFilePath, uploadContext.Id);
                _dbContext.TranscribeDatas.AddRange(list);
                await _dbContext.SaveChangesAsync();

                _responseData.Data = new { videoUrl = VideoUrl(uploadKey), transcribe = list };
                //Delete temporary mp3 audio
                File.Delete(audioFilePath);
                _logger.LogInformation("UploadComplete: Successfully completed upload for uploadKey: {UploadKey}", uploadKey);
            }
            catch (Exception ex)
            {
                _responseData.ErrorMessage = ex.Message;
                _responseData.IsSuccess = false;
                _logger.LogError(ex, "UploadComplete: Error completing upload for uploadKey: {UploadKey}", uploadKey);
            }
            return _responseData;
        }

        private string VideoUrl(string uploadKey)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return baseUrl + "/VideoUpload/StreamVideo/" + uploadKey;
        }

        static string GenerateRandomKey(string allowedChars, int length)
        {
            Random random = new Random();
            StringBuilder keyBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(0, allowedChars.Length);
                char randomChar = allowedChars[randomIndex];
                keyBuilder.Append(randomChar);
            }

            return keyBuilder.ToString();
        }
        private static void MergeChunks(string chunk1, string chunk2)
        {
            FileStream fs1 = null;
            FileStream fs2 = null;
            try
            {
                fs1 = System.IO.File.Open(chunk1, FileMode.Append);
                fs2 = System.IO.File.Open(chunk2, FileMode.Open);
                byte[] fs2Content = new byte[fs2.Length];
                fs2.Read(fs2Content, 0, (int)fs2.Length);
                fs1.Write(fs2Content, 0, (int)fs2.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " : " + ex.StackTrace);
            }
            finally
            {
                if (fs1 != null) fs1.Close();
                if (fs2 != null) fs2.Close();
                System.IO.File.Delete(chunk2);
            }
        }

        public async Task<ResponseContext> StreamVideo(string uploadKey)
        {
            try
            {
                #region check if upload key exists or if uploading has not ended
                var uploadContext = _dbContext.UploadContexts.FirstOrDefault(x => x.UploadKey == uploadKey);
                if (uploadContext == null || uploadContext.isUploading)
                {
                    _responseData.IsSuccess = false;
                    _responseData.ErrorMessage = "Upload key does not exist or the upload has not ended.";
                    return _responseData;
                }
                #endregion

                string fileName = uploadKey;
                string videoFilePath = Path.Combine(tempFolder, fileName); // Adjust the path to your video folder

                if (!File.Exists(videoFilePath))
                {
                    _responseData.IsSuccess = false;
                    _responseData.ErrorMessage = "Video file not found.";
                    return _responseData;
                }

                var stream = File.OpenRead(videoFilePath);

                _responseData.Data = new ResponseWithStreamContext { stream = stream, uploadContext = uploadContext };
                return _responseData;
            }
            catch (Exception ex)
            {
                _responseData.IsSuccess = false;
                _responseData.ErrorMessage = "An error occurred while streaming the video.";
                return _responseData;
            }
        }

        public async Task<ResponseContext> GetAll()
        {
            _logger.LogInformation(Path.GetTempPath());
            var result = await _dbContext.UploadContexts.Include(x => x.TranscribedData).ToListAsync();
            _responseData.Data = result.Select(x => new ResponseUploadContext
            {
                uploadContext = x,
                VideoLink = VideoUrl(x.UploadKey)
            }).ToList();
            return _responseData;
        }
    }

}
