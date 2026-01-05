using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

public class VideoPlaybackController : MonoBehaviour
{
    public Toggle screensizeToggle, fullscreenToggle;

    [SerializeField] private GameObject exitVideoButton;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    private RenderTexture renderTexture;
    private Queue<VideoEntry> queue;
    private VideoConfig config;

    const string screenSize = "Screen Size";
    const string fullScreen = "Full Screen";

    private void Awake()
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.playOnAwake = false;

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.loopPointReached += OnFinished;

        screensizeToggle.isOn = PlayerPrefs.GetInt(screenSize, 0) == 1;
        fullscreenToggle.isOn = PlayerPrefs.GetInt(fullScreen, 0) == 1;
    }

    public void ScreenSizeValueChange(bool val)
    {
        bool isScreen = !screensizeToggle.isOn;

        PlayerPrefs.SetInt(screenSize, isScreen ? 1 : 0);
        SetResizable(isScreen);
        PlayerPrefs.Save();
    }

    public void FullscreenValueChange(bool val)
    {
        bool isFullScreen = fullscreenToggle.isOn;

        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt(fullScreen, isFullScreen ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void Play()
    {
        config = VideoConfigManager.Load();

        if(config.videos.Count == 0)
        {
            WindowsMessageBox.Show("No Data Found!", "Error!");
            return;
        }
        rawImage.gameObject.SetActive(true);
        exitVideoButton.SetActive(true);

        queue = new Queue<VideoEntry>(
            config.videos.OrderBy(v => v.order)
        );

        PlayNext();
    }

    private void PlayNext()
    {
        if (queue.Count == 0)
        {
            if (config.loop)
                Play();
            else
                ExitVideoPlayback();
            return;
        }

        var video = queue.Dequeue();

        string path = Path.Combine(
            Application.streamingAssetsPath,
            "Videos",
            video.fileName
        );

        videoPlayer.url = path;
        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        int w = (int)vp.width;
        int h = (int)vp.height;

        CreateRT(w, h);

        vp.targetTexture = renderTexture;
        rawImage.texture = renderTexture;
        aspectRatioFitter.aspectRatio = (float)w / h;

        vp.Play();
    }

    private void OnFinished(VideoPlayer vp)
    {
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

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    public void ExitVideoPlayback()
    {
        videoPlayer.Stop();
        renderTexture.Release();
        rawImage.gameObject.SetActive(false);
        exitVideoButton.SetActive(false);
    }

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    private const int GWL_STYLE = -16;
    private const int WS_SIZEBOX = 0x00040000; // Resizable border
    private const int WS_MAXIMIZEBOX = 0x00010000; // Maximize button

    public void SetResizable(bool resizable)
    {
        IntPtr windowHandle = GetActiveWindow();
        int style = GetWindowLong(windowHandle, GWL_STYLE);

        if (resizable)
        {
            style |= WS_SIZEBOX | WS_MAXIMIZEBOX;
        }
        else
        {
            style &= ~(WS_SIZEBOX | WS_MAXIMIZEBOX);
        }

        SetWindowLong(windowHandle, GWL_STYLE, style);
    }
}