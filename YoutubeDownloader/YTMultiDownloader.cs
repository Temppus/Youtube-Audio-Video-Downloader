using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace YoutubeDownloader
{
    public class YTMultiDownloader
    {
        #region Settings
        private string ExportVideoDirPath { get; set; }
        private string ExportAudioDirPath { get; set; }
        private ExportOptions ExportOptions { get; set; }
        #endregion

        private IDictionary<Guid, Engine> engines;
        private IDictionary<Guid, VideoDownloader> videoDownloaders;

        private IList<Task<DownloadResult>> tasks;
        private IList<LinkInfo> linksToProcess;

        public void AddDownloadProgressChangedAction(Action<ProgressEventArgs> action, Guid guid)
        {
            videoDownloaders[guid].DownloadProgressChanged += (sender, args) => { action.Invoke(args); };
        }

        public YTMultiDownloader(YTDownloaderBuilder builder, IList<LinkInfo> links)
        {
            Init(builder);

            foreach (var linkInfo in links)
            {
                engines.Add(linkInfo.GUID, new Engine());
                videoDownloaders.Add(linkInfo.GUID, new VideoDownloader());
                linksToProcess.Add(linkInfo);
            }
        }

        public YTMultiDownloader(YTDownloaderBuilder builder, string [] urls)
        {
            Init(builder);

            foreach (var url in urls)
            {
                var linkInfo = new LinkInfo(url);

                engines.Add(linkInfo.GUID, new Engine());
                videoDownloaders.Add(linkInfo.GUID, new VideoDownloader());
                linksToProcess.Add(linkInfo);
            }
        }

        private void Init(YTDownloaderBuilder builder)
        {
            engines = new Dictionary<Guid, Engine>();
            videoDownloaders = new Dictionary<Guid, VideoDownloader>();

            tasks = new List<Task<DownloadResult>>();
            linksToProcess = new List<LinkInfo>();

            this.ExportAudioDirPath = builder.ExportAudioDirPath;
            this.ExportVideoDirPath = builder.ExportVideoDirPath;
            this.ExportOptions = builder.ExportOptions;
        }

        public DownloadResult[] DownloadMulti()
        {
            return ProcessDownloads().Result;
        }

        public async Task<DownloadResult[]> DownloadMultiAsync()
        {
            return await ProcessDownloads();
        }

        private async Task<DownloadResult[]> ProcessDownloads()
        {
            foreach (var linkInfo in linksToProcess)
            {
                Task<DownloadResult> downloadTask = Task.Run(() =>
                {
                    string vidExt;
                    string videoName;

                    string videoFilePath = DownloadVideo(linkInfo, out videoName, out vidExt);

                    if (ExportOptions.HasFlag(ExportOptions.ExportAudio))
                        DownloadAudio(linkInfo, videoFilePath, vidExt);

                    if (ExportOptions.HasFlag(ExportOptions.ExportAudio) && !ExportOptions.HasFlag(ExportOptions.ExportVideo))
                    {
                        File.Delete(videoFilePath);
                    }

                    return new DownloadResult() { VideoSavedFilePath = videoFilePath, GUID = linkInfo.GUID, AudioSavedFilePath = TransFormToAudioPath(videoFilePath, vidExt), FileBaseName = videoName }; ;
                });

                tasks.Add(downloadTask);
            }
            return await Task.WhenAll(tasks);
        }

        private string DownloadVideo(LinkInfo linkInfo, out string videoName, out string videoExtension)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(linkInfo.URL, false);

            // Select the first video by type with highest AudioBitrate
            VideoInfo video = videoInfos
                .OrderByDescending(info => info.AudioBitrate)
                .First(info => info.VideoType == linkInfo.VideoType);

            // This is must, cause we decrypting only this video
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            string fileBaseName = linkInfo.FileName == null ? video.Title.ToSafeFileName() : linkInfo.FileName.ToSafeFileName();
            var filePath = Path.Combine(ExportVideoDirPath, fileBaseName + video.VideoExtension);

            var videoDownloader = videoDownloaders[linkInfo.GUID];

            videoDownloader.Init(video, filePath);
            videoDownloader.Execute();

            videoName = fileBaseName;
            videoExtension = video.VideoExtension;

            // full video path
            return String.Concat(Path.Combine(ExportVideoDirPath, fileBaseName), video.VideoExtension); ;
        }

        private void DownloadAudio(LinkInfo linkInfo, string videoFilePath, string videoExtension)
        {
            string audioOutputPath = TransFormToAudioPath(videoFilePath, videoExtension);

            var inputFile = new MediaFile { Filename = videoFilePath };
            var outputFile = new MediaFile { Filename = audioOutputPath };

            var engine = engines[linkInfo.GUID];

            engine.Convert(inputFile, outputFile);
            engine.Dispose();
        }

        private string TransFormToAudioPath(string videoFilePath, string videoExtension)
        {
            string audioOutputPath = videoFilePath
                                    .Replace(ExportVideoDirPath, ExportAudioDirPath)
                                    .ReplaceLastOccurrence(videoExtension, "mp3");
            return audioOutputPath;
        }
    }
}
