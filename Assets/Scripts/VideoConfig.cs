using System;
using System.Collections.Generic;

[Serializable]
public class VideoEntry
{
    public string fileName;
    public int width;
    public int height;
    public int order=-1;
}

[Serializable]
public class VideoConfig
{
    public List<VideoEntry> videos = new List<VideoEntry>();
}