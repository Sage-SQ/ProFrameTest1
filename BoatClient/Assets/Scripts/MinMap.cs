using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 aaa1 = transform.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            Vector3 bbb = new Vector3(aaa1.x, 0, aaa1.z);
        }
    }
}
