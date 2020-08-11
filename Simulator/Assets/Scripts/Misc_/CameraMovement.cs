using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraMovement : MonoBehaviour
{
    public Slider scaleSlider;
    public TMP_Text scaleText;
    private float m = 0.2f;

    private bool activeMovement;

	// Zoom
	private float minSize = 5f;
	private float maxSize = 71f;
    private float size;
	private float sensitivity = 10f;
	
	// Panning
	private Vector3 mouseOrigin;
	private bool isPanning;
	private float panSpeed = 0.75f;

    private void Awake()
    {
        scaleSlider.onValueChanged.AddListener(delegate { ChangeZoom(); });
        scaleText.text = Mathf.RoundToInt(m * size) + "m";
    }

    void Update()
	{
		if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
		{
			size = Camera.main.orthographicSize;
			size -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
			size = Mathf.Clamp(size, minSize, maxSize);
			Camera.main.orthographicSize = size;

            scaleText.text = Mathf.RoundToInt(m * size) + "m";
            scaleSlider.value = size;
		}
		
		if(activeMovement)
		{
			if(Input.GetMouseButtonDown(0))
			{
				mouseOrigin = Input.mousePosition;
				isPanning = true;
			}
			if (!Input.GetMouseButton(0)) isPanning=false;
			if (isPanning)
			{
				Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
                float speedFactor = size / minSize;
				Vector3 move = new Vector3(pos.x * panSpeed * speedFactor, pos.y * panSpeed * speedFactor, 0);
				transform.Translate(move, Space.Self);
			}
		}
	}

    public void EnableMovement(){ activeMovement = true;}
    public void DisableMovement(){ activeMovement = false;}
	
    private void ChangeZoom()
    {
        size = scaleSlider.value;
        Camera.main.orthographicSize = size;
        scaleText.text = Mathf.RoundToInt(m * size) + "m";
    }
}
