# YoutubeDownloader

## Overview
YoutubeDownloader is small .NET library, written in C# which is capable of dowloading videos from youtube and converting these videos to audio files.
This library is using some code from [YoutubeExtractor](https://github.com/flagbug/YoutubeExtractor) project and uses [MediaToolkit](https://github.com/AydinAdn/MediaToolkit) library as converting tool to convert video to audio file.
Basically it is only wrapper around these to libraries.

## Target platforms

- .NET Framework 4.5 and higher

## Licensing
This project is licensed under MIT license, but use 2 libraries:

- [YoutubeExtractor](https://github.com/flagbug/YoutubeExtractor) library (MIT licensed)
- [MediaToolkit](https://github.com/AydinAdn/MediaToolkit) library which is also licensed under MIT, but uses FFmpeg, a multimedia framework which is licensed
under the [LGPLv2.1 license](http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html), its source can be downloaded
from [here](https://github.com/AydinAdn/MediaToolkit/tree/master/FFMpeg%20src)

## Example code

```c#
string exportVideoPath = @"D:\Downloads\TestExports\Video";
string exportAudioPath = @"D:\Downloads\TestExports\Audio";

var downloader = new YTDownloaderBuilder()
                .SetExportAudioPath(exportAudioPath) // mandatory
                .SetExportVideoPath(exportVideoPath) // mandatory
                .SetExportOptions(ExportOptions.ExportAudio | ExportOptions.ExportVideo) // default ExportOptions.ExportAudio | ExportOptions.ExportVideo
                .Build();

downloader.DownLoad("https://www.youtube.com/watch?v=12345");
downloader.Dispose();
```
##Subscribing to events:
YTDownloader is exposing all event API used YoutubeExtractor and MediaToolkit.

Example
```c#
downloader.DownloadProgressChanged += ...
downloader.DownloadFinished += ...
downloader.DownloadStarted += ...

downloader.ConvertProgressEvent += ...
downloader.ConversionCompleteEvent += ...
```
