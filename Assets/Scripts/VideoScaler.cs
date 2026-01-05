using UnityEngine;
using UnityEngine.Video;

public static class VideoScaler
{
    public static void Adjust(
        VideoPlayer player,
        ref RenderTexture rt,
        int videoW,
        int videoH)
    {
        float videoAspect = (float)videoW / videoH;
        float screenAspect = (float)Screen.width / Screen.height;

        int targetWidth;
        int targetHeight;

        if (videoAspect > screenAspect)
        {
            targetWidth = Screen.width;
            targetHeight = Mathf.RoundToInt(Screen.width / videoAspect);
        }
        else
        {
            targetHeight = Screen.height;
            targetWidth = Mathf.RoundToInt(Screen.height * videoAspect);
        }

        // Recreate RenderTexture safely
        if (rt != null)
        {
            if (rt.width != targetWidth || rt.height != targetHeight)
            {
                rt.Release();
                //Object.Destroy(rt);
                rt = null;
            }
        }

        if (rt == null)
        {
            rt = new RenderTexture(targetWidth, targetHeight, 0)
            {
                name = "VideoRenderTexture"
            };
            rt.Create();
        }

        player.targetTexture = rt;
    }
}
