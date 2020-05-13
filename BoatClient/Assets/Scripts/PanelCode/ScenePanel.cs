using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScenePanel : PanelBase
{
    private RectTransform compass;
    private RectTransform minMap;
    private GameObject minMapCam;
    private Toggle compassBtnT;
    private Toggle minMapBtnT;
    private Toggle testBtnT;
    private Dropdown lookChose;
    private Text UserCountText;

    private float zRotation = 0;
    GameObject camObj;
    Vector3 MinMapPlayerPos = Vector3.zero;
    Dictionary<string,RectTransform> pointList = new Dictionary<string,RectTransform>();
    string IDName = "";

    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "ScenePanel";
        layer = PanelLayer.Panel;

        camObj = Camera.main.gameObject;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        compass = skinTrans.Find("compass").GetComponent<RectTransform>();
        minMap = skinTrans.Find("MinMapBg").GetComponent<RectTransform>();
        minMap.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * 0.3f);//.Set(0,0,Screen.width * 0.3f,Screen.height * 0.3f);
        minMap.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * 0.3f);
        minMapCam = GameObject.Find("MinMapCamera");
        compassBtnT = skinTrans.Find("sidebarPanel").Find("CompassBtn").GetComponent<Toggle>();
        minMapBtnT = skinTrans.Find("sidebarPanel").Find("MinMapBtn").GetComponent<Toggle>();
        testBtnT = skinTrans.Find("sidebarPanel").Find("TestBtn").GetComponent<Toggle>();
        lookChose = skinTrans.Find("sidebarPanel").Find("LookChose").GetComponent<Dropdown>();
        UserCountText = skinTrans.Find("sceneInfo").Find("userCount").GetComponent<Text>();

        //GlobalSetting.list
        foreach (var item in GlobalSetting.list)
        {
            RectTransform point;
            if (item.Value.boat.ctrlType == Boat.CtrlType.player)
            {
                IDName = item.Key;
                point = Instantiate(Resources.Load<RectTransform>("PlayerPointGreen"));
            }
            else
            {
                point = Instantiate(Resources.Load<RectTransform>("PlayerPointRed"));
            }
            point.SetParent(minMap);
            pointList.Add(item.Key, point);
        }
        UserCountText.text = "在线人数：" + GlobalSetting.list.Count + "    本机ID：" + IDName;

        compassBtnT.onValueChanged.AddListener(compassTState);
        minMapBtnT.onValueChanged.AddListener(minMapTState);
        lookChose.onValueChanged.AddListener(lookChoseState);
    }

    //帧更新
    public override void Update()
    {
        zRotation = camObj.transform.eulerAngles.y;

        compass.eulerAngles = new Vector3(0, 0, zRotation);//改变image的Z轴rotation

        foreach (var item in pointList)
        {
            MinMapPlayerPos = minMapCam.GetComponent<Camera>().WorldToViewportPoint(GlobalSetting.list[item.Key].trans.position);
            item.Value.anchoredPosition = new Vector3(MinMapPlayerPos.x * Screen.width * 0.3f, MinMapPlayerPos.y * Screen.height * 0.3f, 0);
        }
    }

    public override void OnClosing()
    {
    }

    public void compassTState(bool value)
    {
        print("compassTState" + value);
        if(value)
        {
            compass.GetComponent<Image>().enabled = true;
        }
        else
        {
            compass.GetComponent<Image>().enabled = false;
        }
    }

    public void minMapTState(bool value)
    {
        print("minMapTState" + value);
        if (value)
        {
            minMap.GetComponent<Image>().enabled = true;
            minMapCam.SetActive(true);
        }
        else
        {
            minMapCam.SetActive(false);
            minMap.GetComponent<Image>().enabled = false;
        }
    }

    public void lookChoseState(int index)
    {
        print("lookChoseState" + index);
        switch (index)
        {
            case 0:
                GlobalSetting.lookMode = 0;
                Camera.main.nearClipPlane = 0.01f;
                break;
            case 1:
                GlobalSetting.lookMode = 1;
                Camera.main.nearClipPlane = 0.3f;
                break;
            case 2:
                GlobalSetting.lookMode = 2;
                Camera.main.nearClipPlane = 0.3f;
                break;
            default:
                GlobalSetting.lookMode = 0;
                Camera.main.nearClipPlane = 0.01f;
                break;
        }
    }
}
