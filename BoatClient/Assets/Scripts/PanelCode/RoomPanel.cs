using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class RoomPanel : PanelBase
{
    private List<Transform> prefabs = new List<Transform>();
    private Button closeBtn;
    private Button startBtn;

    #region 生命周期
    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        //组件
        for (int i = 0; i < 6; i++)
        {
            string name = "PlayerPrefab" + i.ToString();
            Transform prefab = skinTrans.Find(name);
            prefabs.Add(prefab);
        }
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
        //按钮事件
        closeBtn.onClick.AddListener(OnCloseClick);
        startBtn.onClick.AddListener(OnStartClick);
        //监听
        NetMgr.srvConn.msgDist.AddListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.AddListener("Fight", RecvFight);
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        NetMgr.srvConn.Send(protocol);


    }

    public override void OnClosing()
    {

        NetMgr.srvConn.msgDist.DelListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.DelListener("Fight", RecvFight);

    }


    public void RecvGetRoomInfo(ProtocolBase protocol)
    {
        //获取总数
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        //每个处理
        int i = 0;
        for (i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            int team = proto.GetInt(start, ref start);
            int boatModel = proto.GetInt(start, ref start);
            //int win = proto.GetInt(start, ref start);
            //int fail = proto.GetInt(start, ref start);
            int isOwner = proto.GetInt(start, ref start);
            //信息处理
            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            string str = "账号名称：" + id + "\r\n";
            switch(boatModel)
            {
                case 0:
                    str += "船 型 号：智腾号" + "\r\n";
                    break;
                case 1:
                    str += "船 型 号：智腾号green" + "\r\n";
                    break;
                case 2:
                    str += "船 型 号：ship1" + "\r\n";
                    break;
                default:
                    str += "船 型 号：未知类型" + "\r\n";
                    break;
            }
            //str += "阵    营：" + (team == 1 ? "红" : "蓝") + "\r\n";
            //str += "胜利：" + win.ToString() + "   ";
            //str += "失败：" + fail.ToString() + "\r\n";
            if (id == GameMgr.instance.id)
                str += "【我自己】";
            if (isOwner == 1)
                str += "【主机】";
            text.text = str;

            //if (team == 1)
            //    trans.GetComponent<Image>().color = Color.red;
            //else
            //    trans.GetComponent<Image>().color = Color.blue;
        }

        for (; i < 6; i++)
        {
            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            text.text = "【等待接入】";
            trans.GetComponent<Image>().color = Color.gray;
        }
    }

    public void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("LeaveRoom");
        NetMgr.srvConn.Send(protocol, OnCloseBack);
    }


    public void OnCloseBack(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出成功!");
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出失败！");
        }
    }


    public void OnStartClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("StartFight");
        NetMgr.srvConn.Send(protocol, OnStartBack);
    }

    public void OnStartBack(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret != 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "开始模拟失败！只有主机可以开始模拟！");
        }
    }


    public void RecvFight(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        //MultiBattle.instance.StartBattle(proto);
        MultiSimulate.instance.StartBattle(proto);
        PanelMgr.instance.OpenPanel<ScenePanel>("");
        Close();
    }

    #endregion
}