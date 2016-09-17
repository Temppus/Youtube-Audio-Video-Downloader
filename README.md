# Youtube Audio/Video Downloader

## Overview
YoutubeDownloader is .NET library, written in C# which is capable of multiple asynchronous video downloads from youtube and converting these videos to audio files (mp3).
This library is using modified code from [YoutubeExtractor](https://github.com/flagbug/YoutubeExtractor) project and uses [MediaToolkit](https://github.com/AydinAdn/MediaToolkit) library as converting tool to convert video to audio file.

## Target platforms

- .NET Framework 4.5 and higher

## Simple synchronous example

```c#
string exportVideoPath = @"D:\Downloads\TestExports\Video";
string exportAudioPath = @"D:\Downloads\TestExports\Audio";

var link1 = new LinkInfo("https://www.youtube.com/watch?v=123");

var downloader = new YTDownloaderBuilder()
                .SetExportAudioPath(exportAudioPath) // mandatory
                .SetExportVideoPath(exportVideoPath) // mandatory
                .SetExportOptions(ExportOptions.ExportVideo | ExportOptions.ExportAudio) // default setting
                .SetSkipDownloadIfFilesExists(false) // default setting
                .SetLinks(link1) // check other overloads
                .Build();

DownloadResult[] results = downloader.DownloadLinks(); // process download

foreach (var res in results)
{
    Console.WriteLine(res.AudioSavedFilePath);
    Console.WriteLine(res.VideoSavedFilePath);
    Console.WriteLine(res.FileBaseName);
    Console.WriteLine(res.GUID);
    Console.WriteLine(res.DownloadSkipped);
}
```

## Multiple asynchronous file example
```c#

var link1 = new LinkInfo("https://www.youtube.com/watch?v=123");
var link2 = new LinkInfo("https://www.youtube.com/watch?v=456");

var downloader = new YTDownloaderBuilder()
                .SetExportAudioPath(exportAudioPath) // mandatory
                .SetExportVideoPath(exportVideoPath) // mandatory
                .SetExportOptions(ExportOptions.ExportVideo | ExportOptions.ExportAudio) // default setting
                .SetSkipDownloadIfFilesExists(false) // default setting
                .SetLinks(link1, link2) // check other overloads
                .Build();

Task<DownloadResult[]> results = downloader.DownloadLinksAsync(CancellationToken.None); // process download
```
##Subscribing to events:
YTDownloader is exposing all event API used YoutubeExtractor, MediaToolkit library has known bug so we are using custom one.

Example
```c#
downloader.AddDownloadStartedAction(link1.GUID, (evArgs) => { Console.WriteLine("DOWNLOAD STARTED"); });
downloader.AddDownloadFinishedAction(link1.GUID, (evArgs) => { Console.WriteLine("DOWNLOAD FINISHED"); });
downloader.AddDownloadProgressChangedAction(link1.GUID, (progressArgs) =>
{
    Console.WriteLine("Download for link : " + link1.URL + " " + progressArgs.ProgressPercentage + "%");
});

downloader.AddAudioConvertingStartedAction(link1.GUID, (convertArgs) =>
{
    Console.WriteLine("Converting audio to path : " + convertArgs.AudioSavedFilePath);
});

downloader.AddAudioConvertingEndedAction(link1.GUID, (convertArgs) =>
{
    Console.WriteLine("Converting audio done !");
});
```
## Licensing
This project is licensed under MIT license, but use 2 libraries:

- [YoutubeExtractor](https://github.com/flagbug/YoutubeExtractor) library (MIT licensed)
- [MediaToolkit](https://github.com/AydinAdn/MediaToolkit) library which is also licensed under MIT, but uses FFmpeg, a multimedia framework which is licensed
under the [LGPLv2.1 license](http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html), its source can be downloaded
from [here](https://github.com/AydinAdn/MediaToolkit/tree/master/FFMpeg%20src)
