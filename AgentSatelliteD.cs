using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using MBaske.Sensors.Grid;
using MBaske.MLUtil;
using System.Collections.Generic;
using System;



public class AgentSatelliteD : Agent
{
    [HideInInspector]
    public Team team;
    BehaviorParameters m_BehaviorParameters;

    [HideInInspector]
    public Rigidbody agentRb;
    private StatsRecorder m_Stats;

    [SerializeField]
    [Tooltip("Reference to sensor component for retrieving detected opponent gameobjects.")]
    // private GridSensorComponent3D m_SensorComponent;
    
    //private int m_StatsInterval = 120;
    public Vector3 initialPos;
    EnvironmentParameters m_ResetParams;

    public GameObject target;
    public GameObject attacker1;
    public GameObject attacker2;
    public GameObject attacker3;
    public GameObject attacker4;


    public override void Initialize()
    {

        initialPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        // 速度在ResetScene()定义了
        agentRb = GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;

    }


    public void MoveAgent(float []act)
    {  

        float R = 42371000; // 轨道高度  米
        float Miu = 3.98603e14f;//地球常数
        // 参考坐标系的轨道角速度计算
        float w0 = (float)Math.Sqrt(Miu/Math.Pow(R,3));
     
        float posx = this.agentRb.transform.localPosition.x;
        float posy = this.agentRb.transform.localPosition.y;
        float posz = this.agentRb.transform.localPosition.z;

        float velx = agentRb.velocity.x;
        float vely = agentRb.velocity.y;
        float velz = agentRb.velocity.z;
        // 计算三轴加速度
        float Accx = (float)(3*Math.Pow(w0,2)*posx + 2*w0*velz+act[0]);
        float Accy = (float)(-Math.Pow(w0,2)*posy + act[1]);
        float Accz = (float)(-2*w0*velx + act[2]);

        //Debug.Log("m_BehaviorParameters.TeamId:"+m_BehaviorParameters.TeamId+"Accx:"+Accx+"Accy:"+Accy+"Accz:"+Accz);
        //Debug.Log("m_BehaviorParameters.TeamId:"+m_BehaviorParameters.TeamId+"posx:"+posx+"posy:"+posy+"posz:"+posz);

        agentRb.AddForce(Accx,Accy,Accz,
        ForceMode.Acceleration);
    }

    private void AddDecisionRequester()
    {
        Debug.Log("AddDecisionRequester");        
        var req = gameObject.AddComponent<DecisionRequester>();
        req.DecisionPeriod = 5;
        req.TakeActionsBetweenDecisions = true;
    }
 
 // CollectObservations  考虑对手的
    public override void CollectObservations(VectorSensor sensor)
    {
        /*
         // Target and Agent positions  6D
        sensor.AddObservation(attacker.transform.localPosition);  
        sensor.AddObservation(this.transform.localPosition);

        //Target and Agent velocity   6D
        sensor.AddObservation(attacker.GetComponent<Rigidbody>().velocity);
        sensor.AddObservation(agentRb.velocity);
        */

        sensor.AddObservation((this.transform.localPosition - target.transform.localPosition));// maxD /1000f
        sensor.AddObservation((agentRb.velocity - target.GetComponent<Rigidbody>().velocity));// maxv /50f
        sensor.AddObservation((this.transform.localPosition - attacker1.transform.localPosition));// maxD 1000/1000f
        sensor.AddObservation((agentRb.velocity - attacker1.GetComponent<Rigidbody>().velocity));// maxv 50?/50f
        sensor.AddObservation((this.transform.localPosition - attacker2.transform.localPosition));// maxD 1000/1000f
        sensor.AddObservation((agentRb.velocity - attacker2.GetComponent<Rigidbody>().velocity));// maxv 50?/50f
        sensor.AddObservation((this.transform.localPosition - attacker3.transform.localPosition));// maxD 1000/1000f
        sensor.AddObservation((agentRb.velocity - attacker3.GetComponent<Rigidbody>().velocity));// maxv 50?/50f
        sensor.AddObservation((this.transform.localPosition - attacker4.transform.localPosition));// maxD 1000/1000f
        sensor.AddObservation((agentRb.velocity - attacker4.GetComponent<Rigidbody>().velocity));// maxv 50?/50f
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //Debug.Log("heuristic");

        var continuousActionsOut = actionsOut.ContinuousActions;
        
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        //鼠标在垂直方向上的移动
        //continuousActionsOut[2] = Input.GetAxis("Mouse Y");
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[2] = -1;
        }
        /*
        continuousActionsOut[0] = UnityEngine.Random.Range(0.9f, 1f);
        continuousActionsOut[1] = UnityEngine.Random.Range(0.9f, 1f);
        continuousActionsOut[2] = UnityEngine.Random.Range(0.9f, 1f);
        */
             
    }

    //add force to player  可以将轨道坐标系看作固定坐标系？  
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        // 加速度范围  待定
        float[] actions = new float[3];
        actions[0] = 0.02f * actionBuffers.ContinuousActions[0];
        actions[1] = 0.02f * actionBuffers.ContinuousActions[1];
        actions[2] = 0.02f * actionBuffers.ContinuousActions[2];
        // 相当于step,从而获得下一刻状态
        /*Debug.Log("actionsEH:"+actions[0]);
        Debug.Log("actionsEV:"+actions[1]);
        Debug.Log("actionsEAD:"+actions[2]);*/
        MoveAgent(actions);
        
    }


    public override void OnEpisodeBegin()
    {

    }


    

}
