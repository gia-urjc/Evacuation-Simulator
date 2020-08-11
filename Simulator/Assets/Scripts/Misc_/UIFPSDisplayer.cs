using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class UIFPSDisplayer : MonoBehaviour
{
    const float fpsMeasurePeriod = 0.5f;
    private int fpsAccumulator = 0;
    private float fpsNextPeriod = 0;
    private int currentFps;
    const string display = "{0} FPS";
    private TMP_Text text;

    private void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
    }

    private void Start()
    {
        fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        text = GetComponent<TMP_Text>();
    }


    private void Update()
    {
        // measure average frames per second
        fpsAccumulator++;
        if (Time.realtimeSinceStartup > fpsNextPeriod)
        {
            currentFps = (int)(fpsAccumulator / fpsMeasurePeriod);
            fpsAccumulator = 0;
            fpsNextPeriod += fpsMeasurePeriod;
            text.text = string.Format(display, currentFps);
        }
    }
    
}
