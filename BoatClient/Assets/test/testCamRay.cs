using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCamRay : MonoBehaviour
{
    public Transform Ball; //小球(用来标记坐标)

    //设置射线在Plane上的目标点target
    private Vector3 target;

    public Camera cam;

    public RectTransform cambgUI;

    void Start()
    {
        cambgUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * 0.3f);//.Set(0,0,Screen.width * 0.3f,Screen.height * 0.3f);
        cambgUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * 0.3f);
    }

    void Update()
    {
        if (Input.GetMouseButton(1)) //点击鼠标右键
        {
            //object ray = cam.ScreenPointToRay(Input.mousePosition); //屏幕坐标转射线
            Vector3 aaa = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 aaa1 = cam.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("坐标为：" + aaa + "  " + aaa1);
            Vector3 bbb = new Vector3(aaa1.x,0, aaa1.z);
            //Vector3 bbb = new Vector3(10 * Mathf.Tan(Mathf.PI/6) * (aaa.x - 0.5f) / 0.3f,0, 10 * Mathf.Tan(Mathf.PI / 6) * (aaa.y - 0.5f) / 0.3f);
            //Debug.Log("坐标为：" + bbb);
            Ball.position = bbb;
            //RaycastHit hit;                                                     //射线对象是：结构体类型（存储了相关信息）
            //bool isHit = Physics.Raycast((Ray)ray, out hit);             //发出射线检测到了碰撞   isHit返回的是 一个bool值
            //if (isHit)
            //{
            //    Debug.Log("坐标为：" + hit.point);
            //    target = hit.point; //检测到碰撞，就把检测到的点记录下来
            //}
        }
        //如果检测到小球的坐标 与 碰撞的点坐标 距离大于0.1f，就移动小球的位置到 碰撞的点 ：target
        //Ball.position = Vector3.Distance(Ball.position, target) > 0.1f ? Vector3.Lerp(Ball.position, target, Time.deltaTime) : target;

        //Move(target);//以上是Move函数的简写，此函数可不调用
    }
}
