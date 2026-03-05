using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public VideoPlaybackController videoPlaybackController;
    public VideosDataHandler videosData;
    
    public Toggle screensizeToggle, fullscreenToggle, loopToggle;

    public TMP_Dropdown totalVideoDropdown;
    
    [SerializeField] private Transform videoButtonParent;
    [SerializeField] private GameObject videoButton;
    
    [SerializeField] private Transform videoInputParent;
    [SerializeField] private GameObject videoInput;
    [SerializeField] private GameObject videoSettingsData;

    private List<GameObject> videoSettingsDataObjs = new List<GameObject>();
    
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    private const int GWL_STYLE = -16;
    private const int WS_SIZEBOX = 0x00040000; // Resizable border
    private const int WS_MAXIMIZEBOX = 0x00010000; // Maximize button
    
    const string screenSize = "Screen Size";
    const string fullScreen = "Full Screen";
    
    List<Button> videoButtons = new List<Button>();
    List<TMP_InputField> videoInputs = new List<TMP_InputField>();
    
    List<string> durations = new List<string>();

    public List<RectTransform> menuRects;

    void Start()
    {
        screensizeToggle.isOn = PlayerPrefs.GetInt(screenSize, 0) == 0;
        fullscreenToggle.isOn = PlayerPrefs.GetInt(fullScreen, 0) == 1;

        SetVideoButtons();

        totalVideoDropdown.ClearOptions();
        List<string> options = new List<string>();

        int totalVideos = VideoConfigManager.Load().videos.Count+2;
        
        for (int i = 1; i < totalVideos; i++)
        {
            options.Add(i.ToString());
        }
        totalVideoDropdown.AddOptions(options);

        for (int i = 1; i < 61; i++) durations.Add(i.ToString());

        if ((Screen.width + 0.0f)/Screen.height < 2)
        {
            foreach (var rect in menuRects)
            {
                rect.offsetMin = new Vector2(0, rect.offsetMin.y);
            }
        }
    }

    public void SaveVideoNames()
    {
        for (int i = 0; i < videoInputs.Count; i++)
        {
            if(!string.IsNullOrEmpty(videoInputs[i].text)) videosData.SetVideoName(i,videoInputs[i].text);
        }

        SetVideoButtons();
    }

    public void PlayLoopVideos()
    {
        if (!loopToggle.isOn)
        {
            WindowsMessageBox.Show("Loop is off in menu screen!", "Error!");
            return;
        }

        var videosData = VideoConfigManager.Load();
        if (videosData.videos.Count == 0)
        {
            WindowsMessageBox.Show("No videos data available!", "Error!");
            return;
        }
        
        videoPlaybackController.Play(videosData.videos[0].fileName);
    }

    public void UpdateUI()
    {
        SetVideoButtons();
        int totalVideos = VideoConfigManager.Load().videos.Count+2;
        List<string> options = new List<string>();
        for (int i = 1; i < totalVideos; i++)
        {
            options.Add(i.ToString());
        }
        totalVideoDropdown.ClearOptions();
        totalVideoDropdown.AddOptions(options);
    }

    void SetVideoButtons()
    {
        var data = videosData.GetVideosData;
        foreach (var button in videoButtons)
        {
            Destroy(button.gameObject);
        }
        foreach (var input in videoInputs)
        {
            Destroy(input.gameObject);
        }
        
        videoButtons.Clear();
        videoInputs.Clear();
        
        var totalVideos = VideoConfigManager.Load().videos.Count;
        
        for (int i = 0; i < data.videosData.Count; i++)
        {
            var btn = Instantiate(videoButton, videoButtonParent);
            btn.SetActive(true);
            btn.GetComponentInChildren<TMP_Text>().text = data.videosData[i].videoName;
            var button = btn.GetComponent<Button>();
            var file = data.videosData[i].videoFilename;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(()=>videoPlaybackController.Play(file));
            button.interactable = i < totalVideos;
            
            var inp = Instantiate(videoInput, videoInputParent);
            inp.SetActive(true);
            inp.GetComponentInChildren<TMP_Text>().text = data.videosData[i].videoName;
            inp.GetComponent<TMP_InputField>().interactable = i < totalVideos;
            
            videoButtons.Add(btn.GetComponent<Button>());
            videoInputs.Add(inp.GetComponent<TMP_InputField>());
        }
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
    
    public void OnQuitPressed()
    {
        Application.Quit();
    }
    
    public void OnTotalVideosDropdownValueChange(int val)
    {
        int totalVideos = VideoConfigManager.Load().videos.Count;
        var selectedVideos = int.Parse(totalVideoDropdown.options[totalVideoDropdown.value].text);
        if (selectedVideos > totalVideos-1) totalVideoDropdown.value = totalVideos-1;

        foreach (var obj in videoSettingsDataObjs) Destroy(obj);
        
        videoSettingsDataObjs.Clear();
        
        List<string> videoNames = new List<string>();
        foreach (var vd in videosData.GetVideosData.videosData)
        {
            if(!string.IsNullOrEmpty(vd.videoName)) videoNames.Add(vd.videoName);
        }
        
        for (int i = 0; i < selectedVideos; i++)
        {
            var obj = Instantiate(videoSettingsData, videoSettingsData.transform.parent);
            obj.gameObject.SetActive(true);
            obj.GetComponent<VideoSettingsItem>().Init(videoNames,durations);
            
            videoSettingsDataObjs.Add(obj);
        }
    }

    public void SaveVideoSettings()
    {
        foreach (var vd in videosData.GetVideosData.videosData) vd.videoTime = -1;

        foreach (var obj in videoSettingsDataObjs)
        {
            var vi = obj.GetComponent<VideoSettingsItem>();
            videosData.GetVideosData.videosData.Find(x =>
                x.videoName == vi.GetVideoName()).videoTime = vi.GetVideoDuration();
            Debug.Log("Setting Video Duration : "+vi.GetVideoName());
        } 
    }

    public void ClearData()
    {
        foreach (var video in VideoConfigManager.Load().videos)
        {
            string path = Path.Combine(
                Application.streamingAssetsPath,
                "Videos",
                video.fileName
            );
            File.Delete(path);
        }
        VideoConfigManager.ClearData();
        videosData.ClearData();
        
        WindowsMessageBox.Show("All videos data Cleared!", "Message!");
        SetVideoButtons();
    }
}