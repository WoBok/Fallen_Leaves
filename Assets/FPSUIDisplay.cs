using UnityEngine;
using System.Collections;
using Unity.XR.PXR;
using UnityEngine.UI;

public class FPSUIDisplay:MonoBehaviour
{
    private float updateInterval = 1.0f;
    private float timeLeft = 0.0f;
    private string strFps = null;
    Text fpsText;
    private void Start()
    {
        fpsText = GetComponent<Text>();
    }
    void Update()
    {
        ShowFps();
    }

    private void ShowFps()
    {
        timeLeft -= Time.unscaledDeltaTime;

        if (timeLeft <= 0.0)
        {
            float fps = PXR_Plugin.System.UPxr_GetConfigInt(ConfigType.RenderFPS);

            strFps = string.Format("FPS: {0:f0}", fps);

            fpsText.text = strFps;

            timeLeft += updateInterval;
        }
    }
}






