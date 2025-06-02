using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Obi;
public enum Team
{
    Red = 0,
    Blue = 1,
    Green = 2
}

public class SatelliteEnvController : MonoBehaviour
{
    [System.Serializable]
    public class AttackerInfo
    {
        public AgentSatelliteA Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }

    [System.Serializable]
    public class DefenderInfo
    {
        public AgentSatelliteD Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }

    [System.Serializable]
    public class TargetInfo
    {
        public AgentSatelliteT Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }


    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 3000;

    //List of Agents On Platform
    public List<AttackerInfo> AttackerList = new List<AttackerInfo>();
    public List<TargetInfo> TargetList = new List<TargetInfo>();
    public List<DefenderInfo> DefenderList = new List<DefenderInfo>();


    private SimpleMultiAgentGroup m_RedAttackerGroup;
    private SimpleMultiAgentGroup m_BlueTargetGroup;
    private SimpleMultiAgentGroup m_GreenDefenderGroup;

    private int m_ResetTimer;
    private int resetCounter = 0;

    public ObiSolverInitializer obiSolverInitializer;

    public ClothCollisionDetector collisionDetector;



    // 在ResetScene方法外部声明一个变量来保存上一局的初始位置
    private Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();

    void Start()
    {
        m_RedAttackerGroup = new SimpleMultiAgentGroup();
        m_BlueTargetGroup = new SimpleMultiAgentGroup();
        m_GreenDefenderGroup = new SimpleMultiAgentGroup();

        foreach (var item in AttackerList)
        {
            item.StartingPos = item.Agent.transform.localPosition;
            item.StartingRot = item.Agent.transform.localRotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            m_RedAttackerGroup.RegisterAgent(item.Agent);
        }
        foreach (var item in TargetList)
        {
            item.StartingPos = item.Agent.transform.localPosition;
            item.StartingRot = item.Agent.transform.localRotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            m_BlueTargetGroup.RegisterAgent(item.Agent);
        }

        foreach (var item in DefenderList)
        {
            item.StartingPos = item.Agent.transform.localPosition;
            item.StartingRot = item.Agent.transform.localRotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            m_GreenDefenderGroup.RegisterAgent(item.Agent);
        }


        ResetScene();
        obiSolverInitializer.InitializeObiSolverPosition();

    }
    // 10 次决策一次  中间的动作值是没有还是决策量

    void FixedUpdate()
    {


        int collisionResult = collisionDetector.GetCollisionResult();


        var agentTar = TargetList[0];

        Vector3 target_Pos = agentTar.Agent.transform.localPosition;


        m_ResetTimer += 1;

        var agentred1 = AttackerList[0];
        var agentred2 = AttackerList[1];
        var agentred3 = AttackerList[2];
        var agentred4 = AttackerList[3];

        var agentgreen = DefenderList[0];

        // 过程奖励和终端奖励
        // 过程奖励   距离 速度夹角？
        Vector3 posagentred1 = agentred1.Agent.transform.localPosition;
        Vector3 posagentgreen = agentgreen.Agent.transform.localPosition;
        Vector3 posagentred2 = agentred2.Agent.transform.localPosition;
        Vector3 posagentred3 = agentred3.Agent.transform.localPosition;
        Vector3 posagentred4 = agentred4.Agent.transform.localPosition;

        Vector3 RedCenter = (posagentred1 + posagentred2 + posagentred3 + posagentred4) / 4;

        // 追踪者中心距离目标的距离
        Vector3 relaPos0 = target_Pos - RedCenter;
        float distancePE0 = relaPos0.magnitude;

        // defender defenderTar 距离
        Vector3 relaPosEE = target_Pos - posagentgreen;
        float disEE = relaPosEE.magnitude;


        // 追踪者与障碍物不能发生碰撞   距离至少相差20
        Vector3 relaPosPB = posagentgreen - RedCenter;
        float distancePB = relaPosPB.magnitude;

        Vector3 relaPosP1B = posagentgreen - posagentred1;  // 单个航天器距离至少大于5
        float distanceP1B = relaPosP1B.magnitude;
        Vector3 relaPosP2B = posagentgreen - posagentred2;  // 单个航天器距离至少大于5
        float distanceP2B = relaPosP2B.magnitude;
        Vector3 relaPosP3B = posagentgreen - posagentred3;  // 单个航天器距离至少大于5
        float distanceP3B = relaPosP3B.magnitude;
        Vector3 relaPosP4B = posagentgreen - posagentred4;  // 单个航天器距离至少大于5
        float distanceP4B = relaPosP4B.magnitude;

        // persuer 之间的距离
        Vector3 relaPos12 = posagentred1 - posagentred2;
        Vector3 relaPos13 = posagentred1 - posagentred3;
        Vector3 relaPos14 = posagentred1 - posagentred4;
        Vector3 relaPos23 = posagentred2 - posagentred3;
        Vector3 relaPos24 = posagentred2 - posagentred4;
        Vector3 relaPos34 = posagentred3 - posagentred4;

        float distanceP12 = relaPos12.magnitude;
        float distanceP13 = relaPos13.magnitude;
        float distanceP14 = relaPos14.magnitude;
        float distanceP23 = relaPos23.magnitude;
        float distanceP24 = relaPos24.magnitude;
        float distanceP34 = relaPos34.magnitude;

   
        int maxPB = 2; // 假设1000为最大可能距离
        int edgeD = 8;
        float edgeDcl = 11.3124f;
        float R_danger = 0;
        float Re_step = 0;
        float ReT_step = 0;

        //float R_D_angle = Vector3.Dot(relaPosPB, relaPosEE) / distancePB / disEE;   //D在TA中间-1，最小，所以加个负号
        //float R_T_angle = Vector3.Dot(relaPos0, relaPosEE) / distancePE0 / disEE;

        // attacker 
        float R_form = -Mathf.Abs(distanceP12 - edgeD) - Mathf.Abs(distanceP13 - edgeDcl) - Mathf.Abs(distanceP14 - edgeD) - Mathf.Abs(distanceP23 - edgeD) - Mathf.Abs(distanceP24 - edgeDcl) - Mathf.Abs(distanceP34 - edgeD);
        float Rstep = -distancePE0 /30 + R_form / 20;

        // ABF 斥力
        if (distancePB <= 8)
        {
            Rstep = Rstep  -  5 / distancePB;
        }

        // defender（障碍物）
        Re_step = -distancePB / 30;   //  - distancePB / 30 +;0.5f - R_D_angle;
                     // target(逃逸)

        ReT_step = distancePE0 / 30; //+m_ResetTimer/ MaxEnvironmentSteps/10  - R_D_angle

        //if (disEE <= 8)
        //{
        //    Re_step = Re_step - 8 / disEE;
        //    ReT_step = ReT_step - 8 / disEE;
        //}


        m_RedAttackerGroup.AddGroupReward(Rstep);
        m_GreenDefenderGroup.AddGroupReward(Re_step);
        m_BlueTargetGroup.AddGroupReward(ReT_step);

        double capD = 4;  // 捕获界限


        if (distancePE0 <= capD && collisionResult == 1)                                                                    //    Form_Centroid_disRstep2_V2
                                                                                                    //if (R_form >= -12) 
        {
            m_RedAttackerGroup.AddGroupReward(3000);   //1 - (float)m_ResetTimer / MaxEnvironmentSteps    500-(float)m_ResetTimer / MaxEnvironmentSteps*200
            m_BlueTargetGroup.AddGroupReward(-500);
            m_GreenDefenderGroup.AddGroupReward(-500);  //m_BlueTargetGroup.AddGroupReward(0);//-1
            m_RedAttackerGroup.GroupEpisodeInterrupted();
            m_BlueTargetGroup.GroupEpisodeInterrupted();
            m_GreenDefenderGroup.GroupEpisodeInterrupted();
            ResetScene();
            obiSolverInitializer.InitializeObiSolverPosition();
            Debug.Log("1");
        }

        // 碰撞结束?
        if (disEE <= 2)                                                                    //    Form_Centroid_disRstep2_V2
        {
            m_RedAttackerGroup.AddGroupReward(0);//1 - (float)m_ResetTimer / MaxEnvironmentSteps    500-(float)m_ResetTimer / MaxEnvironmentSteps*200
            m_BlueTargetGroup.AddGroupReward(-2000);                                    //m_BlueTargetGroup.AddGroupReward(0);//-1
            m_GreenDefenderGroup.AddGroupReward(-2000);
            m_GreenDefenderGroup.GroupEpisodeInterrupted();
            m_RedAttackerGroup.GroupEpisodeInterrupted();
            m_BlueTargetGroup.GroupEpisodeInterrupted();
            ResetScene();
            obiSolverInitializer.InitializeObiSolverPosition();
            Debug.Log("2");
        }

        // 四个航天器设置不会发生碰撞如何,因为不想再设置他们之间碰撞的惩罚了，太繁琐了
        if (collisionResult == 2 && distancePB < 4)                                                                    //    Form_Centroid_disRstep2_V2
                                                                                                  
        {
            m_RedAttackerGroup.AddGroupReward(-3000);//1 - (float)m_ResetTimer / MaxEnvironmentSteps    500-(float)m_ResetTimer / MaxEnvironmentSteps*200
            m_BlueTargetGroup.AddGroupReward(1000);     // 我只是想僵持着                               //m_BlueTargetGroup.AddGroupReward(0);//-1
            m_GreenDefenderGroup.AddGroupReward(3000);
            m_GreenDefenderGroup.GroupEpisodeInterrupted();
            m_RedAttackerGroup.GroupEpisodeInterrupted();
            m_BlueTargetGroup.GroupEpisodeInterrupted();
            obiSolverInitializer.InitializeObiSolverPosition();
            ResetScene();
            Debug.Log("4");
        }

        // 未捕获到结束判断
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_RedAttackerGroup.AddGroupReward(0);   //-distancePE/maxD      - distancePE0*4 - 20
            m_BlueTargetGroup.AddGroupReward(1000);   //1+ distancePE/maxD
            m_GreenDefenderGroup.AddGroupReward(3000);
            m_GreenDefenderGroup.GroupEpisodeInterrupted();
            m_BlueTargetGroup.GroupEpisodeInterrupted();
            m_RedAttackerGroup.GroupEpisodeInterrupted();
            ResetScene();
            obiSolverInitializer.InitializeObiSolverPosition();
            Debug.Log("5");

        }



    }


    public void ResetScene()
    {
        m_ResetTimer = 0;

        //Debug.Log("m_ResetTimer:" + m_ResetTimer);
        int resetFrequency = 1;
        resetCounter++;

        foreach (var item in AttackerList)
        {

            var randomPosX = Random.Range(-1f, 1f);
            var randomPosY = Random.Range(-1f, 1f);
            var randomPosZ = Random.Range(-1f, 1f);
            //Debug.Log("StartingPos:" + newStartPos);
            var newStartPos = item.Agent.initialPos;// + new Vector3(randomPosX, randomPosY, randomPosZ);
            var newRot = Quaternion.Euler(0, 0, 0);
            item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }
        foreach (var item in TargetList)
        {
            // 定义环形带的内外半径
            float innerRadius = 20f;
            float outerRadius = 30f;

            // 在内外半径之间随机选择一个半径
            float randomRadius = Random.Range(innerRadius, outerRadius);

            // 随机选择一个角度
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);

            // 计算 X 和 Z 坐标
            float randomPosX = randomRadius * Mathf.Cos(randomAngle);
            float randomPosZ = randomRadius * Mathf.Sin(randomAngle);

            // Y 轴坐标保持不变
            float randomPosY = 0f;

            // 创建新的起始位置
            var newStartPosT = item.Agent.initialPos + new Vector3(randomPosX, randomPosY, randomPosZ);
            //var newStartPosT = item.Agent.initialPos + new Vector3(0, 0, 0);

            // 定义旋转（保持原始旋转）
            var newRot = Quaternion.Euler(0, 0, 0);

            // 设置新的位置和旋转
            item.Agent.transform.SetPositionAndRotation(newStartPosT, newRot);

            // 清零速度和角速度
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;

        }

        foreach (var item in DefenderList)
        {
            //var randomPosX = RandomRangeSelector(-8f, -4f, 4f, 8f);
            //var randomPosY = RandomRangeSelector(-10f, -4f, 4f, 10f);
            //var randomPosZ = RandomRangeSelector(-6f, -4f, 4f, 6f);

            ////var newStartPosE = item.Agent.initialPos  + new Vector3(randomPosX, randomPosY, randomPosZ);
            //var newStartPosE = item.Agent.initialPos + new Vector3(randomPosX, 0, randomPosZ);
            //var newRot = Quaternion.Euler(0, 0, 0);
            //item.Agent.transform.SetPositionAndRotation(newStartPosE, newRot);
            //item.Rb.velocity = Vector3.zero;
            //item.Rb.angularVelocity = Vector3.zero;

            float innerRadius = 15f;
            float outerRadius = 20f;

            // 在内外半径之间随机选择一个半径
            float randomRadius = Random.Range(innerRadius, outerRadius);

            // 随机选择一个角度
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);

            // 计算 X 和 Z 坐标
            float randomPosX = randomRadius * Mathf.Cos(randomAngle);
            float randomPosZ = randomRadius * Mathf.Sin(randomAngle);

            // Y 轴坐标保持不变
            float randomPosY = 0f;

            // 创建新的起始位置
            var newStartPosE = item.Agent.initialPos + new Vector3(randomPosX, randomPosY, randomPosZ);
            //var newStartPosE = item.Agent.initialPos + new Vector3(0, 0, 0);

            // 定义旋转（保持原始旋转）
            var newRot = Quaternion.Euler(0, 0, 0);

            // 设置新的位置和旋转
            item.Agent.transform.SetPositionAndRotation(newStartPosE, newRot);

            // 清零速度和角速度
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }
    }

    float RandomRangeSelector(float min1, float max1, float min2, float max2)
    {
        // 生成0或1的随机整数来选择区间
        return Random.Range(0, 2) == 0 ? Random.Range(min1, max1) : Random.Range(min2, max2);
    }
}
