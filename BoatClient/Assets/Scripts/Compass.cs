using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    private float zRotation = 0;
    GameObject camObj;
    // Start is called before the first frame update
    void Start()
    {
        camObj = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        zRotation = camObj.transform.eulerAngles.y;

        transform.eulerAngles = new Vector3(0, 0, zRotation);//改变image的Z轴rotation
    }
}
