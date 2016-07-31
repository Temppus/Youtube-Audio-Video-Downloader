using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace YoutubeDownloader
{
    public class LinkInfo
    {
        public string URL { get; private set; }
        public Guid GUID { get; private set; }
        public string FileName { get; private set; }
        public VideoType VideoType { get; private set; }

        public LinkInfo(string url, string fileNameToBeSaved = null, VideoType videoType = VideoType.Mp4)
        {
            this.GUID = Guid.NewGuid();
            this.URL = url;
            this.FileName = fileNameToBeSaved;
            this.VideoType = videoType;
        }
    }
}
