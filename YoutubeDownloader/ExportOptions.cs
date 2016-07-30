using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader
{
    [Flags]
    public enum ExportOptions
    {
        ExportVideo = 1,
        ExportAudio = 2
    }
}
