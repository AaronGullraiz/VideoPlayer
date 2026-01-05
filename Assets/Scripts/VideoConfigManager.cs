using System.IO;
using UnityEngine;

public static class VideoConfigManager
{
    private static string ConfigPath =>
        Path.Combine(Application.streamingAssetsPath, "video_config.json");

    public static VideoConfig Load()
    {
        if (!File.Exists(ConfigPath))
            return new VideoConfig();

        string json = File.ReadAllText(ConfigPath);
        return JsonUtility.FromJson<VideoConfig>(json);
    }

    public static void Save(VideoConfig config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(ConfigPath, json);
    }
}
