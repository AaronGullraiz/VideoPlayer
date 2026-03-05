using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPlaybackController : MonoBehaviour
{
    public VideosDataHandler videosDataHandler;
    [SerializeField] private GameObject exitVideoButton;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;
    public GameObject videoImage;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    private RenderTexture renderTexture;
    private List<VideoEntry> playingVideosData=new();
    private VideoConfig config;
    
    bool isLoop = false;

    private float playingVideoTime = 0;
    int playingVideoIndex = 0;

    private void Awake()
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.playOnAwake = false;

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.loopPointReached += OnFinished;
    }

    public void Play(string filename)
    {
        config = VideoConfigManager.Load();

        if(config.videos.Count == 0)
        {
            WindowsMessageBox.Show("No Data Found!", "Error!");
            return;
        }
        videoImage.SetActive(true);
        exitVideoButton.SetActive(true);

        playingVideosData.Clear();
        isLoop = PlayerPrefs.GetInt("IsLooping",0)==1;
        if (isLoop)
        {
            for (int i = 0; i < videosDataHandler.GetVideosData.videosData.Count; i++)
            {
                if (videosDataHandler.GetVideosData.videosData[i].videoTime > 0)
                {
                    playingVideosData.Add(config.videos.Find(x=>x.fileName == videosDataHandler.GetVideosData.videosData[i].videoFilename));
                }
            }
            playingVideoIndex = playingVideosData.FindIndex(x=>x.fileName == filename);
        }
        else
        {
            playingVideosData.Add(config.videos.Find(x=>x.fileName == filename));
            playingVideoIndex = 0;
        }

        PlayNext();
    }

    private void PlayNext()
    {
        if (playingVideosData.Count == 0) return;
        
        var video = playingVideosData[playingVideoIndex];

        string path = Path.Combine(
            Application.streamingAssetsPath,
            "Videos",
            video.fileName
        );

        videoPlayer.isLooping = isLoop;
        videoPlayer.url = path;
        videoPlayer.Prepare();
        playingVideoTime = videosDataHandler.GetVideoTime(video.fileName);
    }

    private void OnPrepared(VideoPlayer vp)
    {
        int w = (int)vp.width;
        int h = (int)vp.height;

        CreateRT(w, h);

        vp.targetTexture = renderTexture;
        rawImage.texture = renderTexture;
        aspectRatioFitter.aspectRatio = (float)w / h;

        vp.isLooping = true;
        vp.Play();
        if(isLoop) StartCoroutine(VideoTimer(playingVideoTime));
    }

    private void OnFinished(VideoPlayer vp)
    {
        if(!isLoop) ExitVideoPlayback();
    }

    IEnumerator VideoTimer(float time)
    {
        yield return new WaitForSeconds(time*60);
        playingVideoIndex = (playingVideoIndex+1) % playingVideosData.Count;
        PlayNext();
    }

    private void CreateRT(int w, int h)
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        renderTexture = new RenderTexture(w, h, 0);
        renderTexture.Create();
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }

    public void ExitVideoPlayback()
    {
        videoPlayer.Stop();
        renderTexture.Release();
        videoImage.SetActive(false);
        exitVideoButton.SetActive(false);
        StopAllCoroutines();
        playingVideosData.Clear();
        playingVideoIndex = 0;
    }
}