using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiSolver))]
public class ClothCollisionDetector : MonoBehaviour
{
    public ObiCloth cloth; // 布料对象
    public GameObject attacker1; // 攻击者1
    public GameObject attacker2; // 攻击者1
    public GameObject attacker3; // 攻击者1
    public GameObject attacker4; // 攻击者1
    public GameObject target; // 目标
    public GameObject defender; // 防守者

    private ObiSolver solver;
    private int collisionResult; // 用于存储碰撞结果

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

        // 如果距离大于7，就跳过碰撞检测
        if (distance > 6f && distance1 > 6f)
        {
            //Debug.Log(distance1);
            return;
        }

        collisionResult = 0; // 重置碰撞结果
        var world = ObiColliderWorld.GetInstance();

        foreach (Oni.Contact contact in e.contacts)
        {
            // 检查实际碰撞
            if (contact.distance < 0.01)
            {
                // 获取参与碰撞的粒子索引
                int particleIndex = solver.simplices[contact.bodyA];

                // 获取粒子所属的actor
                ObiSolver.ParticleInActor particleInActor = solver.particleToActor[particleIndex];

                // 检查粒子是否属于布
                if (particleInActor.actor == cloth)
                {
                    ObiColliderBase col = world.colliderHandles[contact.bodyB].owner;

                    if (col != null)
                    {
                        // 检查是否与刚体A或刚体B发生碰撞
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

    // 用于获取碰撞结果
    public int GetCollisionResult()
    {
        return collisionResult;
    }
}
