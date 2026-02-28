using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VideoSettingsItem : MonoBehaviour
{
    public TMP_Dropdown videoNamesDropdown, durationDropdown;

    public void Init(List<string> videoNames, List<string> durations)
    {
        videoNamesDropdown.ClearOptions();
        durationDropdown.ClearOptions();
        videoNamesDropdown.AddOptions(videoNames);
        durationDropdown.AddOptions(durations);
    }

    public float GetVideoDuration()
    {
        return int.Parse(durationDropdown.options[durationDropdown.value].text);
    }
    
    public string GetVideoName()
    {
        return videoNamesDropdown.options[videoNamesDropdown.value].text;
    }
}