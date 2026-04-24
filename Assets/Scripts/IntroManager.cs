using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public Vector2 screenSize;
    
    void Start()
    {
        Screen.SetResolution(Mathf.RoundToInt(screenSize.x), Mathf.RoundToInt(screenSize.y), FullScreenMode.FullScreenWindow);
        
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate(); // second monitor
        }
        
        SceneManager.LoadScene(1);
    }
}