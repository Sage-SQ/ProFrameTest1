using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSimulate : MonoBehaviour
{
    //单例
    public static MultiSimulate instance;
    //船预设
    public GameObject[] boatPrefabs;
    //场景中所有船只
    public Dictionary<string, SimulateBoat> list = new Dictionary<string, SimulateBoat>();

    // Use this for initialization
    void Start()
    {
        //单例模式
        instance = this;
    }

    //获取阵营 0表示错误
    public int GetCamp(GameObject boatObj)
    {
        foreach (SimulateBoat mt in list.Values)
        {
            if (mt.boat.gameObject == boatObj)
                return mt.camp;
        }
        return 0;
    }

    //是否同一阵营
    public bool IsSameCamp(GameObject boat1, GameObject boat2)
    {
        return GetCamp(boat1) == GetCamp(boat2);
    }

    //清理场景
    public void ClearBattle()
    {
        list.Clear();
        GameObject[] boats = GameObject.FindGameObjectsWithTag("Boat");
        for (int i = 0; i < boats.Length; i++)
            Destroy(boats[i]);
    }

    //开始模拟
    public void StartBattle(ProtocolBytes proto)
    {
        //解析协议
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (protoName != "Fight")
            return;
        //船总数
        int count = proto.GetInt(start, ref start);
        //清理场景
        ClearBattle();
        //每一艘船
        for (int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            int team = proto.GetInt(start, ref start);
            int boatModel = proto.GetInt(start, ref start);
            int swopID = proto.GetInt(start, ref start);
            GenerateTank(id, team, boatModel, swopID);
        }
        NetMgr.srvConn.msgDist.AddListener("UpdateUnitInfo", RecvUpdateUnitInfo);
        //NetMgr.srvConn.msgDist.AddListener ("Shooting", RecvShooting);
        //NetMgr.srvConn.msgDist.AddListener ("Hit", RecvHit);
        //NetMgr.srvConn.msgDist.AddListener ("Result", RecvResult);
    }

    //产生船
    public void GenerateTank(string id, int team,int boatModel, int swopID)
    {
        //获取出生点
        Transform sp = GameObject.Find("SwopPoints").transform;
        Transform swopTrans;
        if (team == 1)
        {
            Transform teamSwop = sp.GetChild(0);
            swopTrans = teamSwop.GetChild(swopID - 1);
        }
        else
        {
            Transform teamSwop = sp.GetChild(1);
            swopTrans = teamSwop.GetChild(swopID - 1);
        }
        if (swopTrans == null)
        {
            Debug.LogError("GenerateBoat出生点错误！");
            return;
        }
        //预设
        if (boatPrefabs.Length < 2)
        {
            Debug.LogError("船预设数量不够");
            return;
        }
        //产生船
        GameObject boatObj = (GameObject)Instantiate(boatPrefabs[boatModel]);
        boatObj.name = id;
        boatObj.transform.position = swopTrans.position;
        boatObj.transform.rotation = swopTrans.rotation;
        //列表处理
        SimulateBoat sb = new SimulateBoat();
        sb.boat = boatObj.GetComponent<Boat>();
        sb.camp = team;
        list.Add(id, sb);
        //用户处理
        if (id == GameMgr.instance.id)
        {
            sb.boat.ctrlType = Boat.CtrlType.player;
            //CameraFollow cf = Camera.main.gameObject.GetComponent<CameraFollow>();
            //GameObject target = sb.boat.gameObject;
            //cf.SetTarget(target);
        }
        else
        {
            sb.boat.ctrlType = Boat.CtrlType.net;
            sb.boat.InitNetCtrl();  //初始化网络同步
        }
    }


    public void RecvUpdateUnitInfo(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        Vector3 nPos;
        Vector3 nRot;
        nPos.x = proto.GetFloat(start, ref start);
        nPos.y = proto.GetFloat(start, ref start);
        nPos.z = proto.GetFloat(start, ref start);
        nRot.x = proto.GetFloat(start, ref start);
        nRot.y = proto.GetFloat(start, ref start);
        nRot.z = proto.GetFloat(start, ref start);
        float turretY = proto.GetFloat(start, ref start);
        float gunX = proto.GetFloat(start, ref start);
        //处理
        Debug.Log("RecvUpdateUnitInfo " + id);
        if (!list.ContainsKey(id))
        {
            Debug.Log("RecvUpdateUnitInfo bt == null ");
            return;
        }
        SimulateBoat sb = list[id];
        if (id == GameMgr.instance.id)
            return;

        sb.boat.NetForecastInfo(nPos, nRot);
    }
}
