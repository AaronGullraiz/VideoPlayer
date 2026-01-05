using System.IO;
using UnityEngine;
using SFB;
using TMPro;
using UnityEngine.UI;

public class VideoInsertController : MonoBehaviour
{
    public TMP_InputField widthText, heightText;

    public Toggle loopToogle;

    private void Start()
    {
        var config = VideoConfigManager.Load();
        loopToogle.isOn = config.loop;
    }

    public void SelectVideo()
    {
        if (string.IsNullOrEmpty(widthText.text) || string.IsNullOrEmpty(heightText.text)) 
        {
            WindowsMessageBox.Show("Width or Height is empty", "Error!");
            return; 
        }
        PickVideo(int.Parse(widthText.text), int.Parse(heightText.text));
    }

    public void PickVideo(int width, int height)
    {
        var paths = StandaloneFileBrowser.OpenFilePanel(
            "Select Video", "", "*", false);

        if (paths.Length == 0)
            return;

        string sourcePath = paths[0];
        string fileName = Path.GetFileName(sourcePath);

        string targetDir = Path.Combine(Application.streamingAssetsPath, "Videos");
        Directory.CreateDirectory(targetDir);

        string targetPath = Path.Combine(targetDir, fileName);
        File.Copy(sourcePath, targetPath, true);

        var config = VideoConfigManager.Load();

        config.videos.Add(new VideoEntry
        {
            fileName = fileName,
            width = width,
            height = height,
            order = config.videos.Count
        });

        VideoConfigManager.Save(config);
    }

    public void SetLoop(bool loop)
    {
        var config = VideoConfigManager.Load();
        config.loop = loopToogle.isOn;
        VideoConfigManager.Save(config);
    }
}