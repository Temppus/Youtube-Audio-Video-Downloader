using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader
{
    public class AudioConvertingEventArgs : EventArgs
    {
        public Guid GUID { get; internal set; }
        public string AudioSavedFilePath { get; internal set; }
    }
}
