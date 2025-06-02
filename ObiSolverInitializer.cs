using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Obi;

public class ObiSolverInitializer : MonoBehaviour
{
    public ObiSolver obiSolver; // �������Ѿ�ͨ��Inspector����븳ֵ���������
    public Vector3 initialPosition; // ��ϣ��ObiSolver��ʼ������λ��
    //initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

    //Debug.Log("ObiSolverInitializer in 1?"); ���÷���
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
            // ��ObiSolver��Ѱ��ObiCloth���
            ObiCloth obiCloth = obiSolver.GetComponentInChildren<ObiCloth>();
            if (obiCloth != null)
            {
                //Vector3 startPosition = this.transform.position; // ʹ�õ�ǰѵ�������λ����Ϊ��ʼλ��
                //Debug.Log("startPosition:" + startPosition);
                //obiCloth.transform.position = startPosition;
                // ���ҵ���ObiCloth���г�ʼ��
                obiCloth.ResetParticles();
            }
        }
    }
}
