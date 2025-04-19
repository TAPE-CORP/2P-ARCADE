using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class OptimalPathFollower2D : MonoBehaviour
{
    [Header("추적 대상 (플레이어2)")]
    public Transform target;

    [Header("이동 속도")]
    public float moveSpeed = 4f;

    [Header("경로 갱신 간격 (초)")]
    public float pathUpdateInterval = 0.5f;

    private Rigidbody2D rb;
    private NavMeshPath navPath;
    private List<Vector2> corners = new List<Vector2>();
    private int currentCorner = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        navPath = new NavMeshPath();
    }

    void Start()
    {
        // 시작 위치가 NavMesh 위에 없으면 가장 가까운 지점으로 이동
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        else
        {
            Debug.LogWarning("NavMesh 위에 시작점을 찾지 못했습니다.");
        }

        if (target == null)
        {
            Debug.LogError("추적 대상이 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        StartCoroutine(UpdatePathRoutine());
    }

    private IEnumerator UpdatePathRoutine()
    {
        var wait = new WaitForSecondsRealtime(pathUpdateInterval);
        while (true)
        {
            // NavMesh 상의 시작점과 목표점 샘플링
            NavMeshHit startHit, endHit;
            bool hasStart = NavMesh.SamplePosition(transform.position, out startHit, 1f, NavMesh.AllAreas);
            bool hasEnd = NavMesh.SamplePosition(target.position, out endHit, 1f, NavMesh.AllAreas);
            if (hasStart && hasEnd)
            {
                if (NavMesh.CalculatePath(startHit.position, endHit.position, NavMesh.AllAreas, navPath))
                {
                    UpdateCorners();
                }
            }
            else
            {
                Debug.LogWarning("NavMesh 상에서 시작 또는 목표 지점을 찾지 못했습니다.");
            }

            yield return wait;
        }
    }

    private void UpdateCorners()
    {
        corners.Clear();
        foreach (var corner in navPath.corners)
        {
            corners.Add(new Vector2(corner.x, corner.y));
        }
        currentCorner = 1;
    }

    void FixedUpdate()
    {
        if (corners == null || currentCorner >= corners.Count)
            return;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = corners[currentCorner];
        Vector2 direction = (targetPos - currentPos).normalized;
        Vector2 nextPos = currentPos + direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(nextPos);

        if (Vector2.Distance(currentPos, targetPos) < 0.1f)
        {
            currentCorner++;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 부딪힌 객체가 target이면 target 파괴
        if (collision.gameObject == target.gameObject)
        {
            Destroy(target.gameObject);
            Destroy(this.gameObject);
        }
    }
}
