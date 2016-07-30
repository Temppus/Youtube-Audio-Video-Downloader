using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace YoutubeDownloader
{
    public class YTDownloader : IDisposable
    {
        #region Settings
        private string ExportVideoDirPath { get; set; }
        private string ExportAudioDirPath { get; set; }
        private ExportOptions ExportOptions { get; set; }
        #endregion

        #region AudioEngineAPI
        public event EventHandler<ConversionCompleteEventArgs> ConversionCompleteEvent
        {
            add { audioEngine.ConversionCompleteEvent += value; }
            remove { audioEngine.ConversionCompleteEvent -= value; }
        }

        public event EventHandler<ConvertProgressEventArgs> ConvertProgressEvent
        {
            add { audioEngine.ConvertProgressEvent += value; }
            remove { audioEngine.ConvertProgressEvent -= value; }
        }
        #endregion

        #region VideoDownloaderAPI
        public event EventHandler<ProgressEventArgs> DownloadProgressChanged
        {
            add { videoDownloader.DownloadProgressChanged += value; }
            remove { videoDownloader.DownloadProgressChanged -= value; }
        }

        public event EventHandler DownloadStarted
        {
            add { videoDownloader.DownloadStarted += value; }
            remove { videoDownloader.DownloadStarted -= value; }
        }

        public event EventHandler DownloadFinished
        {
            add { videoDownloader.DownloadFinished += value; }
            remove { videoDownloader.DownloadFinished -= value; }
        }
        #endregion

        private Engine audioEngine;
        private VideoDownloader videoDownloader;

        public YTDownloader(YTDownloaderBuilder builder)
        {
            audioEngine = new Engine();
            videoDownloader = new VideoDownloader();

            this.ExportAudioDirPath = builder.ExportAudioDirPath;
            this.ExportVideoDirPath = builder.ExportVideoDirPath;
            this.ExportOptions = builder.ExportOptions;
        }

        public Task DownLoadAsync(string ytLink, string fileName = null, VideoType videoType = VideoType.Mp4)
        {
            return Task.Run(() =>
            {
                DownLoad(ytLink, fileName, videoType);
            });
        }

        public void DownLoad(string ytLink, string fileName = null, VideoType videoType = VideoType.Mp4)
        {
            var videoFilePath = DownloadVideo(ytLink, fileName, videoType);

            if (ExportOptions.HasFlag(ExportOptions.ExportAudio))
                DownloadAudio(videoFilePath);

            if (ExportOptions.HasFlag(ExportOptions.ExportAudio) && !ExportOptions.HasFlag(ExportOptions.ExportVideo))
            {
                File.Delete(videoFilePath);
            }
        }

        private string DownloadVideo(string ytLink, string fileName, VideoType videoType)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(ytLink);

            /*
             * Select the first video by type with highest AudioBitrate
             */
            VideoInfo video = videoInfos
                .OrderByDescending(info => info.AudioBitrate)
                .First(info => info.VideoType == videoType);

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            string fileBaseName = fileName == null ? video.Title.ToSafeFileName() : fileName.ToSafeFileName();
            var filePath = Path.Combine(ExportVideoDirPath, fileBaseName + video.VideoExtension);

            videoDownloader.Init(video, filePath);
            videoDownloader.Execute();

            return String.Concat(Path.Combine(ExportVideoDirPath, fileBaseName), video.VideoExtension);
        }

        private void DownloadAudio(string videoFilePath)
        {
            string audioOutputPath = videoFilePath
                                    .Replace(ExportVideoDirPath, ExportAudioDirPath)
                                    .ReplaceLastOccurrence("mp4", "mp3");

            var inputFile = new MediaFile { Filename = videoFilePath };
            var outputFile = new MediaFile { Filename = audioOutputPath };

            using (var engine = new Engine())
            {
                engine.Convert(inputFile, outputFile);
            }
        }

        public void Dispose()
        {
            audioEngine.Dispose();
        }
    }
}
