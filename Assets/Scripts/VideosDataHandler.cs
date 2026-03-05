using UnityEngine;

public class VideosDataHandler : MonoBehaviour
{
    VideosData data;
    
    const string VideosDataKey = "VideosData";
    
    private void Awake()
    {
        data = new VideosData();
        var val = PlayerPrefs.GetString(VideosDataKey,JsonUtility.ToJson(data));
        data = JsonUtility.FromJson<VideosData>(val);
    }

    public int GetVideosSize=>data.videosData.Count;

    public VideosData GetVideosData => data;

    public void ClearData()
    {
        data = new VideosData();
        PlayerPrefs.SetString(VideosDataKey, JsonUtility.ToJson(data));
    }

    public void AddNewVideo(string filename)
    {
        VideoData d = new();
        d.videoFilename =  filename;
        d.videoName = filename;
        d.videoTime = 1;

        for (int i = 0; i < data.videosData.Count; i++)
        {
            if (data.videosData[i].videoFilename == string.Empty)
            {
                data.videosData[i] = d;
                break;
            }
        }
        
        var val = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(VideosDataKey, val);
        PlayerPrefs.Save();
    }
    
    public void SetVideoName(int index, string videoName)
    {
        data.videosData[index].videoName = videoName;

        var val = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(VideosDataKey, val);
        PlayerPrefs.Save();
    }

    public float GetVideoTime(string fileName)
    {
        float time = 0;

        foreach (var v in data.videosData)
        {
            if (v.videoFilename == fileName)
            {
                time = v.videoTime;
                break;
            }
        }
        
        return  time;
    }
}