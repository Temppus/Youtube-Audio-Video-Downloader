using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader
{
    public class YTDownloaderBuilder
    {
        public string ExportVideoDirPath { get; set; }
        public string ExportAudioDirPath { get; set; }
        public ExportOptions ExportOptions { get; set; }
        public bool SkipVideosWhichExists { get; set; }

        public YTDownloaderBuilder()
        {
            this.ExportOptions = ExportOptions.ExportVideo | ExportOptions.ExportAudio;
            this.ExportAudioDirPath = null;
            this.ExportVideoDirPath = null;
            this.SkipVideosWhichExists = false;
        }

        public YTDownloader Build()
        {
            CheckDirPath(ExportAudioDirPath);
            CheckDirPath(ExportVideoDirPath);

            return new YTDownloader(this);
        }

        public YTMultiDownloader BuildMulti(IList<LinkInfo> links)
        {
            CheckDirPath(ExportAudioDirPath);
            CheckDirPath(ExportVideoDirPath);

            return new YTMultiDownloader(this, links);
        }

        public YTMultiDownloader BuildMulti(params string[] urls)
        {
            CheckDirPath(ExportAudioDirPath);
            CheckDirPath(ExportVideoDirPath);

            return new YTMultiDownloader(this, urls);
        }

        public YTDownloaderBuilder SetExportOptions(ExportOptions opt)
        {
            this.ExportOptions = opt;
            return this;
        }

        public YTDownloaderBuilder SetExportAudioPath(string path)
        {
            this.ExportAudioDirPath = path;
            return this;
        }

        public YTDownloaderBuilder SetExportVideoPath(string path)
        {
            this.ExportVideoDirPath = path;
            return this;
        }

        public YTDownloaderBuilder SetSkipDownloadIfFilesExists(bool skipDownloadIfVideosExists)
        {
            this.SkipVideosWhichExists = skipDownloadIfVideosExists;
            return this;
        }

        public void CheckDirPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new InvalidOperationException("Path to export directory was not set !");

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Directory path {0} is invalid : ", path) + ex.Message);
            }
        }
    }
}
