using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Obi;

public class ObiSolverInitializer : MonoBehaviour
{
    public ObiSolver obiSolver; // 假设你已经通过Inspector或代码赋值了这个引用
    public Vector3 initialPosition; // 你希望ObiSolver初始化到的位置
    //initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

    //Debug.Log("ObiSolverInitializer in 1?"); 不让放这
    void Start()
    {
        InitializeObiSolverPosition();
    }

    public void InitializeObiSolverPosition()
    {
        //Debug.Log("ObiSolverInitializer in 2?");
        //if (obiSolver != null)
        //{
        //    obiSolver.transform.position = initialPosition;
        //}
        //ObiSolver obiSolver = this.gameObject.GetComponentInChildren<ObiSolver>();
        if (obiSolver != null)
        {
            // 在ObiSolver下寻找ObiCloth组件
            ObiCloth obiCloth = obiSolver.GetComponentInChildren<ObiCloth>();
            if (obiCloth != null)
            {
                //Vector3 startPosition = this.transform.position; // 使用当前训练区域的位置作为初始位置
                //Debug.Log("startPosition:" + startPosition);
                //obiCloth.transform.position = startPosition;
                // 对找到的ObiCloth进行初始化
                obiCloth.ResetParticles();
            }
        }
    }
}
