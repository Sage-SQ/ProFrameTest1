using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    GameObject camObj;

    public List<AxleInfo> axleInfos;
    //马力/最大马力
    private float motor = 0;
    public float maxMotorTorque;
    //制动/最大制动
    private float brakeTorque = 0;
    public float maxBrakeTorque = 100;
    //转向角/最大转向角
    private float steering = 0;
    public float maxSteeringAngle;

    //马达音源
    public AudioSource motorAudioSource;
    //马达音效
    public AudioClip motorClip;

    //网络同步
    private float lastSendInfoTime = float.MinValue;

    //操控类型
    public enum CtrlType
    {
        none,
        player,
        computer,
        net,
    }

    public CtrlType ctrlType = CtrlType.player;

    //人工智能
    private BoatAI ai;

    //last 上次的位置信息
    Vector3 lPos;
    Vector3 lRot;
    //forecast 预测的位置信息
    Vector3 fPos;
    Vector3 fRot;
    //时间间隔
    float delta = 1;
    //上次接收的时间
    float lastRecvInfoTime = float.MinValue;

    //位置预测
    public void NetForecastInfo(Vector3 nPos, Vector3 nRot)
    {
        //预测的位置
        fPos = lPos + (nPos - lPos) * 2;
        fRot = lRot + (nRot - lRot) * 2;
        if (Time.time - lastRecvInfoTime > 0.3f)
        {
            fPos = nPos;
            fRot = nRot;
        }
        //时间
        delta = Time.time - lastRecvInfoTime;
        //更新
        lPos = nPos;
        lRot = nRot;
        lastRecvInfoTime = Time.time;
    }

    //初始化位置预测数据
    public void InitNetCtrl()
    {
        lPos = transform.position;
        lRot = transform.eulerAngles;
        fPos = transform.position;
        fRot = transform.eulerAngles;
        Rigidbody r = GetComponent<Rigidbody>();
        r.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void NetUpdate()
    {
        //当前位置
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        //更新位置
        if (delta > 0)
        {
            transform.position = Vector3.Lerp(pos, fPos, delta);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(rot),
                                              Quaternion.Euler(fRot), delta);
        }
        ////炮塔旋转
        //TurretRotation();
        //TurretRoll();
        ////轮子履带马达音效
        //NetWheelsRotation();
    }

    public void NetTurretTarget(float y, float x)
    {
        //turretRotTarget = y;
        //turretRollTarget = x;
    }

    struct Position
    {
        public Vector3 forwardAmount;
        public float turnAmount;
        public Vector2 camRotation;
        public float camDistance;
    }

    public Vector3 m_forward = new Vector3(0, 0, 1);
    public float m_shipMoveSpeed = 20.0f;

    [Range(0.01f, 1.0f)]
    public float shipSmoothness = 0.5f;

    public float m_camRotationSpeed = 10.0f;
    public float m_camStartRotationX = 180.0f;
    public float m_camStartRotationY = 60.0f;
    public float m_camStartDistance = 100.0f;

    [Range(0.01f, 1.0f)]
    public float camSmoothness = 0.5f;

    Position m_position, m_target;

    const float MAX_ACCELERATION = 1.0f;
    const float ACCELERATION_RATE = 1.0f;
    const float DECELERATION_RATE = 0.25f;
    const float ANGULAR_ACC_RATE = 2.0f;

    float m_acceleration = 0.0f;
    Vector3 m_previousPos, m_velocity;


    //用户控制
    public void PlayerCtrl()
    {
        //只有用户操控的船才会生效
        if (ctrlType != CtrlType.player)
            return;
        ////马力和转向角
        //motor = maxMotorTorque * Input.GetAxis("Vertical");
        //steering = maxSteeringAngle * Input.GetAxis("Horizontal");


        float speed = m_shipMoveSpeed;
        float velocity = m_velocity.magnitude;

        m_target.forwardAmount = Vector3.zero;
        m_target.turnAmount = 0.0f;

        if(Input.GetKey(KeyCode.A))
        {
            float deg = speed * velocity;
            m_target.turnAmount -= deg * Time.deltaTime * ANGULAR_ACC_RATE;
            m_target.camRotation.x -= deg * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            float deg = speed * velocity;
            m_target.turnAmount += deg * Time.deltaTime * ANGULAR_ACC_RATE;
            m_target.camRotation.x += deg * Time.deltaTime;
        }
        Vector3 forward = transform.localToWorldMatrix * m_forward;
        forward.Normalize();
        if(Input.GetKey(KeyCode.W))
        {
            m_acceleration += Time.deltaTime * ACCELERATION_RATE;
        }
        else
        {
            m_acceleration -= Time.deltaTime * DECELERATION_RATE;
        }
        m_acceleration = Mathf.Clamp(m_acceleration,0.0f,MAX_ACCELERATION);
        m_target.forwardAmount += forward * speed * m_acceleration * Time.deltaTime;

        float dt = Time.deltaTime * 1000.0f;
        float amount = Mathf.Pow(1.02f, Mathf.Min(dt, 1.0f));

        if(Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            m_target.camDistance *= amount;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            m_target.camDistance /= amount;
        }
        m_target.camDistance = Mathf.Max(1.0f, m_target.camDistance);
        m_target.camRotation.y = Mathf.Clamp(m_target.camRotation.y, 20.0f, 160.0f);

        if(Input.GetMouseButton(0))
        {
            m_target.camRotation.y += Input.GetAxis("Mouse Y") * m_camRotationSpeed;
            m_target.camRotation.x += Input.GetAxis("Mouse X") * m_camRotationSpeed;
        }

        //
        float smoothness;
        smoothness = 1.0f / Mathf.Clamp(camSmoothness,0.01f,1.0f);
        float camLerp = Mathf.Clamp01(Time.deltaTime * smoothness);

        smoothness = 1.0f / Mathf.Clamp(shipSmoothness,0.01f,1.0f);
        float shipLerp = Mathf.Clamp01(Time.deltaTime * smoothness);

        m_position.camDistance = Mathf.Lerp(m_position.camDistance, m_target.camDistance, camLerp);
        m_position.camRotation = Vector2.Lerp(m_position.camRotation,m_target.camRotation,camLerp);
        m_position.forwardAmount = Vector3.Lerp(m_position.forwardAmount,m_target.forwardAmount,shipLerp);
        m_position.turnAmount = Mathf.Lerp(m_position.turnAmount, m_target.turnAmount, shipLerp);

        //
        transform.position += m_position.forwardAmount;
        Vector3 eulerAngels = transform.eulerAngles;
        eulerAngels.y += m_position.turnAmount;
        transform.eulerAngles = eulerAngels;

        float ct = Mathf.Cos(m_position.camRotation.y * Mathf.Deg2Rad);
        float st = Mathf.Sin(m_position.camRotation.y * Mathf.Deg2Rad);
        float cp = Mathf.Cos(m_position.camRotation.x * Mathf.Deg2Rad);
        float sp = Mathf.Sin(m_position.camRotation.x * Mathf.Deg2Rad);

        Vector3 lookAt = transform.position;
        Vector3 pos = lookAt + (new Vector3(sp * st, ct, cp * st)) * m_position.camDistance;

        camObj.transform.position = pos;
        camObj.transform.LookAt(lookAt);

        m_velocity = transform.position - m_previousPos;
        m_previousPos = transform.position;


        ////制动
        //brakeTorque = 0;
        //foreach (AxleInfo axleInfo in axleInfos)
        //{
        //    if (axleInfo.leftWheel.rpm > 5 && motor < 0)  //前进时，按下“下”键
        //        brakeTorque = maxBrakeTorque;
        //    else if (axleInfo.leftWheel.rpm < -5 && motor > 0)  //后退时，按下“上”键
        //        brakeTorque = maxBrakeTorque;
        //    continue;
        //}

        ////炮塔炮管角度
        //TargetSignPos();
        ////发射炮弹
        //if (Input.GetMouseButton(0))
        //    Shoot();

        //网络同步
        if (Time.time - lastSendInfoTime > 0.2f)
        {
            SendUnitInfo();
            lastSendInfoTime = Time.time;
        }
    }

    //电脑控制
    public void CombuterCtrl()
    {
        if (ctrlType != CtrlType.computer)
            return;

        ////炮塔方位
        //Vector3 rot = ai.GetTurretTarget();
        //turretRotTarget = rot.y;
        //turretRollTarget = rot.x;
        ////发射炮弹
        //if (ai.IsShoot())
        //    Shoot();

        ////移动
        //steering = ai.GetSteering();
        //motor = ai.GetMotor();
        //brakeTorque = ai.GetBrakeTorque();
    }

    //无人控制
    public void NoneCtrl()
    {
        if (ctrlType != CtrlType.none)
            return;
        motor = 0;
        steering = 0;
        brakeTorque = maxBrakeTorque / 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        camObj = Camera.main.gameObject;

        ////获取炮塔
        //turret = transform.Find("turret");
        ////获取炮管
        //gun = turret.Find("gun");
        ////获取轮子
        //wheels = transform.Find("wheels");
        ////获取履带
        //tracks = transform.Find("tracks");
        //马达音源
        motorAudioSource = gameObject.AddComponent<AudioSource>();
        motorAudioSource.spatialBlend = 1;
        ////发射音源
        //shootAudioSource = gameObject.AddComponent<AudioSource>();
        //shootAudioSource.spatialBlend = 1;
        //人工智能
        if (ctrlType == CtrlType.computer)
        {
            ai = gameObject.AddComponent<BoatAI>();
            ai.boat = this;
        }


        m_position.camRotation.x = m_camStartRotationX;
        m_position.camRotation.y = m_camStartRotationY;
        m_position.camDistance = m_camStartDistance;
        m_position.forwardAmount = Vector3.zero;
        m_target = m_position;
        m_previousPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //网络同步
        if (ctrlType == CtrlType.net)
        {
            NetUpdate();
            return;
        }
        //操控
        PlayerCtrl();
        CombuterCtrl();
        NoneCtrl();
        //遍历车轴
        foreach (AxleInfo axleInfo in axleInfos)
        {
            ////转向
            //if (axleInfo.steering)
            //{
            //    axleInfo.leftWheel.steerAngle = steering;
            //    axleInfo.rightWheel.steerAngle = steering;
            //}
            ////马力
            //if (axleInfo.motor)
            //{
            //    axleInfo.leftWheel.motorTorque = motor;
            //    axleInfo.rightWheel.motorTorque = motor;
            //}
            ////制动
            //if (true)
            //{
            //    axleInfo.leftWheel.brakeTorque = brakeTorque;
            //    axleInfo.rightWheel.brakeTorque = brakeTorque;
            //}
            ////转动轮子履带
            //if (axleInfos[1] != null && axleInfo == axleInfos[1])
            //{
            //    WheelsRotation(axleInfos[1].leftWheel);
            //    TrackMove();
            //}
        }

        ////炮塔炮管旋转
        //TurretRotation();
        //TurretRoll();
        //马达音效
        MotorSound();
    }

    //马达音效
    void MotorSound()
    {
        if (motor != 0 && !motorAudioSource.isPlaying)
        {
            motorAudioSource.loop = true;
            motorAudioSource.clip = motorClip;
            motorAudioSource.Play();
        }
        else if (motor == 0)
        {
            motorAudioSource.Pause();
        }
    }

    public void SendUnitInfo()
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("UpdateUnitInfo");
        //位置旋转
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        proto.AddFloat(pos.x);
        proto.AddFloat(pos.y);
        proto.AddFloat(pos.z);
        proto.AddFloat(rot.x);
        proto.AddFloat(rot.y);
        proto.AddFloat(rot.z);
        ////炮塔
        //float angleY = turretRotTarget;
        //proto.AddFloat(angleY);
        ////炮管
        //float angleX = turretRollTarget;
        //proto.AddFloat(angleX);
        NetMgr.srvConn.Send(proto);
    }
}
