using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiSolver))]
public class ClothCollisionDetector : MonoBehaviour
{
    public ObiCloth cloth; // ���϶���
    public GameObject attacker1; // ������1
    public GameObject attacker2; // ������1
    public GameObject attacker3; // ������1
    public GameObject attacker4; // ������1
    public GameObject target; // Ŀ��
    public GameObject defender; // ������

    private ObiSolver solver;
    private int collisionResult; // ���ڴ洢��ײ���

    void Awake()
    {
        solver = GetComponent<ObiSolver>();
    }

    void OnEnable()
    {
        solver.OnCollision += Solver_OnCollision;
    }

    void OnDisable()
    {
        solver.OnCollision -= Solver_OnCollision;
    }

    void Solver_OnCollision(object sender, Obi.ObiSolver.ObiCollisionEventArgs e)
    {


        Vector3 RedCenter = (attacker1.transform.localPosition + attacker2.transform.localPosition + attacker3.transform.localPosition + attacker4.transform.localPosition) / 4;
        float distance = Vector3.Distance(RedCenter, target.transform.localPosition);
        float distance1 = Vector3.Distance(RedCenter, defender.transform.localPosition);

        // ����������7����������ײ���
        if (distance > 6f && distance1 > 6f)
        {
            //Debug.Log(distance1);
            return;
        }

        collisionResult = 0; // ������ײ���
        var world = ObiColliderWorld.GetInstance();

        foreach (Oni.Contact contact in e.contacts)
        {
            // ���ʵ����ײ
            if (contact.distance < 0.01)
            {
                // ��ȡ������ײ����������
                int particleIndex = solver.simplices[contact.bodyA];

                // ��ȡ����������actor
                ObiSolver.ParticleInActor particleInActor = solver.particleToActor[particleIndex];

                // ��������Ƿ����ڲ�
                if (particleInActor.actor == cloth)
                {
                    ObiColliderBase col = world.colliderHandles[contact.bodyB].owner;

                    if (col != null)
                    {
                        // ����Ƿ������A�����B������ײ
                        if (col.gameObject == target)
                        {
                            collisionResult = 1;
                            return;
                        }
                        else if (col.gameObject == defender)
                        {
                            collisionResult = 2;
                            return;
                        }
                    }
                }
            }
        }
    }

    // ���ڻ�ȡ��ײ���
    public int GetCollisionResult()
    {
        return collisionResult;
    }
}
