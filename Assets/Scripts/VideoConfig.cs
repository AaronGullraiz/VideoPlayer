using System;
using System.Collections.Generic;

[Serializable]
public class VideoEntry
{
    public string fileName;
    public int width;
    public int height;
    public int order;
}

[Serializable]
public class VideoConfig
{
    public bool loop;
    public List<VideoEntry> videos = new List<VideoEntry>();
}