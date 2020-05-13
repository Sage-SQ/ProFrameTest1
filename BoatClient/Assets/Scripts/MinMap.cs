using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMap : MonoBehaviour
{
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 screenPos = cam.ScreenToViewportPoint(Input.mousePosition);
            if(screenPos.x < 1f && screenPos.y < 1f)
            {
                Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
                GlobalSetting.MapToWorldPos = pos;
                GlobalSetting.isMapClick = true;
            }
        }
    }
}
