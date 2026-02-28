using System.Collections.Generic;

[System.Serializable]
public class VideoData
{
    public string videoName = "";
    public string videoFilename;
    public float videoTime;
}

[System.Serializable]
public class VideosData
{
    public List<VideoData> videosData;

    public VideosData()
    {
        videosData = new List<VideoData>();
        foreach (var vd in VideoConfigManager.Load().videos)
        {
            var videoData = new VideoData();
            videoData.videoName = vd.fileName;
            videoData.videoFilename = vd.fileName;
            videoData.videoTime = 1;
            videosData.Add(videoData);
        }
        for (int i = videosData.Count; i < 10; i++) videosData.Add(new VideoData());
    }
}