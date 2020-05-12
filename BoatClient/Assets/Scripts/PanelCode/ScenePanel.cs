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

    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "ScenePanel";
        layer = PanelLayer.Panel;
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

        compassBtnT.onValueChanged.AddListener(compassTState);
        minMapBtnT.onValueChanged.AddListener(minMapTState);
        lookChose.onValueChanged.AddListener(lookChoseState);
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
                break;
            case 1:
                GlobalSetting.lookMode = 1;
                break;
            case 2:
                GlobalSetting.lookMode = 2;
                break;
            default:
                GlobalSetting.lookMode = 0;
                break;
        }
    }
}
