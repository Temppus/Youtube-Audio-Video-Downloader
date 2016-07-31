using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader
{
    public class DownloadResult
    {
        public string VideoSavedFilePath { get; set; }
        public string AudioSavedFilePath { get; set; }
        public string FileBaseName { get; set; }
        public Guid GUID { get; set; }
    }
}
