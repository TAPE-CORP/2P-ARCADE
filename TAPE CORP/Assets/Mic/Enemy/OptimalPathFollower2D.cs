using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody2D))]
public class OptimalPathFollower2D : MonoBehaviour
{
    [Header("���� ��� (�÷��̾�2)")]
    public Transform target;

    [Header("�̵� �ӵ�")]
    public float moveSpeed = 4f;

    private NavMeshAgent agent;
    private Rigidbody2D rb;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();

        // 2D ���� ����
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false;      // ���� ��ġ ����ȭ
        agent.speed = moveSpeed;
        agent.baseOffset = 0f;         // XY ��� ���̿� ����

        // ��ü�� ���� ��� �ʿ信 ���� ����
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        // NavMesh ���� ������ ���� ��ġ�� Warp
        if (!agent.isOnNavMesh)
            Debug.LogWarning("NavMeshAgent is not on NavMesh. Warping to current position.");
    }

    void Update()
    {
        if (target != null && agent.isOnNavMesh)
        {
            // �� ������ �ִܰ�� ��� ��û
            agent.SetDestination(target.position);
        }
    }

    void FixedUpdate()
    {
        // NavMeshAgent�� ����� ��ġ�� Rigidbody2D�� ���󰡰� ����ȭ
        Vector2 next = new Vector2(agent.nextPosition.x, agent.nextPosition.y);
        rb.MovePosition(Vector2.MoveTowards(rb.position, next, moveSpeed * Time.fixedDeltaTime));
        agent.nextPosition = transform.position;
    }
}
