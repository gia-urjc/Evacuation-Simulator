using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerToggle : MonoBehaviour
{
    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.cullingMask = -1;
    }

    public void ToggleLayer()
    {
        if (cam.cullingMask == -1) cam.cullingMask = 1335;
        else if (cam.cullingMask == 823) cam.cullingMask = 311;
        else if (cam.cullingMask == 1335) cam.cullingMask = -1;
        else cam.cullingMask = 823;
    }

    public void ToggleTileLayer()
    {
        if (cam.cullingMask == -1) cam.cullingMask = 823;
        else if (cam.cullingMask == 823) cam.cullingMask = -1;
        else if (cam.cullingMask == 1335) cam.cullingMask = 311;
        else cam.cullingMask = 1335;
    }
}
