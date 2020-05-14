using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2DUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GlobalSetting.list.Count > 0)
        {
            transform.position = GlobalSetting.list["123"].trans.position;
        }
        
    }
}
