using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatAI : MonoBehaviour
{
    //所控制的船
    public Boat boat;

    //状态
    public enum Status
    {
        Patrol,
        Attack,
    }
    private Status status = Status.Patrol;

    //更改状态
    public void ChangeStatus(Status status)
    {
        if (status == Status.Patrol)
            PatrolStart();
        else if (status == Status.Attack)
            AttackStart();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (boat.ctrlType != Boat.CtrlType.computer)
            return;

        //TargetUpdate();
        //行走
        if (path.IsReach(transform))
        {
            path.NextWaypoint();
        }

        if (status == Status.Patrol)
            PatrolUpdate();
        else if (status == Status.Attack)
            AttackUpdate();
    }

    void OnDrawGizmos()
    {
        path.DrawWaypoints();
    }

    //巡逻开始
    void PatrolStart()
    {

    }


    //攻击开始
    void AttackStart()
    {
        
    }

    //巡逻中
    void PatrolUpdate()
    {
        
    }

    //攻击中
    void AttackUpdate()
    {
        
    }

    //----------------行走状态机----------------------

    //路径
    private Path path = new Path();
    //上次更新路径时间
    private float lastUpdateWaypointTime = float.MinValue;
    //更新路径cd
    private float updateWaypointtInterval = 10; 

    //初始化路点
    void InitWaypoint()
    {
        GameObject obj = GameObject.Find("WaypointContainer");
        if (obj && obj.transform.GetChild(0) != null)
        {
            Vector3 targetPos = obj.transform.GetChild(0).position;
            path.InitByNavMeshPath(transform.position, targetPos);
        }
    }

    ////获取转向角
    //public float GetSteering()
    //{
    //    if (tank == null)
    //        return 0;

    //    Vector3 itp = transform.InverseTransformPoint(path.waypoint);
    //    if (itp.x > path.deviation / 5)
    //        return tank.maxSteeringAngle;
    //    else if (itp.x < -path.deviation / 5)
    //        return -tank.maxSteeringAngle;
    //    else
    //        return 0;
    //}

    ////获取马力
    //public float GetMotor()
    //{

    //    if (tank == null)
    //        return 0;

    //    Vector3 itp = transform.InverseTransformPoint(path.waypoint);
    //    float x = itp.x;
    //    float z = itp.z;
    //    float r = 6;

    //    if (z < 0 && Mathf.Abs(x) < -z && Mathf.Abs(x) < r)
    //        return -tank.maxMotorTorque;
    //    else
    //        return tank.maxMotorTorque;
    //}

    ////获取刹车
    //public float GetBrakeTorque()
    //{
    //    if (path.isFinish)
    //        return tank.maxMotorTorque;
    //    else
    //        return 0;
    //}
}
