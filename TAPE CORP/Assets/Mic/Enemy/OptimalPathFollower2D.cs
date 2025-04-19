using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody2D))]
public class OptimalPathFollower2D : MonoBehaviour
{
    [Header("추적 대상 (플레이어2)")]
    public Transform target;

    [Header("이동 속도")]
    public float moveSpeed = 4f;

    private NavMeshAgent agent;
    private Rigidbody2D rb;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();

        // 2D 전용 설정
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false;      // 직접 위치 동기화
        agent.speed = moveSpeed;
        agent.baseOffset = 0f;         // XY 평면 높이에 맞춤

        // 유체감 낮출 경우 필요에 따라 조절
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        // NavMesh 위에 없으면 현재 위치로 Warp
        if (!agent.isOnNavMesh)
            Debug.LogWarning("NavMeshAgent is not on NavMesh. Warping to current position.");
    }

    void Update()
    {
        if (target != null && agent.isOnNavMesh)
        {
            // 매 프레임 최단경로 계산 요청
            agent.SetDestination(target.position);
        }
    }

    void FixedUpdate()
    {
        // NavMeshAgent가 계산한 위치를 Rigidbody2D로 따라가게 동기화
        Vector2 next = new Vector2(agent.nextPosition.x, agent.nextPosition.y);
        rb.MovePosition(Vector2.MoveTowards(rb.position, next, moveSpeed * Time.fixedDeltaTime));
        agent.nextPosition = transform.position;
    }
}
