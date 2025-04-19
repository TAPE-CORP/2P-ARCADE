using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class OptimalPathFollower2D : MonoBehaviour
{
    [Header("추적 대상 (2개까지)")]
    [Tooltip("순서대로 할당하세요. 하나씩 따라갑니다.")]
    public List<Transform> targets = new List<Transform>(2);

    [Header("이동 속도")]
    public float moveSpeed = 4f;

    [Header("경로 갱신 간격 (초)")]
    public float pathUpdateInterval = 0.5f;

    private Transform currentTarget;
    private Rigidbody2D rb;
    private NavMeshPath navPath;
    private List<Vector2> corners = new List<Vector2>();
    private int currentCorner = 0;
    private Coroutine pathRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        navPath = new NavMeshPath();
    }

    void Start()
    {
        currentTarget = PickNextTarget(null);
        if (currentTarget == null)
        {
            Debug.LogError("추적할 타겟이 없습니다.");
            enabled = false;
            return;
        }

        StickToNavMesh();
        pathRoutine = StartCoroutine(UpdatePathRoutine());
    }

    private IEnumerator UpdatePathRoutine()
    {
        var wait = new WaitForSecondsRealtime(pathUpdateInterval);
        while (true)
        {
            if (currentTarget != null)
            {
                NavMeshHit startHit, endHit;
                bool hasStart = NavMesh.SamplePosition(transform.position, out startHit, 1f, NavMesh.AllAreas);
                bool hasEnd = NavMesh.SamplePosition(currentTarget.position, out endHit, 1f, NavMesh.AllAreas);
                if (hasStart && hasEnd && NavMesh.CalculatePath(startHit.position, endHit.position, NavMesh.AllAreas, navPath))
                {
                    UpdateCorners();
                }
            }
            yield return wait;
        }
    }

    private void UpdateCorners()
    {
        corners.Clear();
        foreach (var corner in navPath.corners)
            corners.Add(new Vector2(corner.x, corner.y));
        currentCorner = 1;
    }

    void FixedUpdate()
    {
        if (corners.Count == 0 || currentCorner >= corners.Count || currentTarget == null)
            return;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = corners[currentCorner];
        Vector2 direction = (targetPos - currentPos).normalized;

        if (direction.sqrMagnitude > 0.0001f)
            transform.right = direction;

        Vector2 nextPos = currentPos + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);

        if (Vector2.Distance(currentPos, targetPos) < 0.1f)
            currentCorner++;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentTarget != null && collision.transform == currentTarget)
        {
            Destroy(currentTarget.gameObject);
            // 다음 타겟 요청
            currentTarget = PickNextTarget(currentTarget);
            corners.Clear();
        }
    }

    /// <summary>
    /// 이전에 제공된 previousTarget을 제외하고, 리스트에서 살아있는(== null 아님) 첫 타겟을 반환합니다.
    /// </summary>
    private Transform PickNextTarget(Transform previousTarget)
    {
        // 리스트 정리: null 참조 제거
        targets.RemoveAll(t => t == null);
        if (targets.Count == 0)
            return null;

        // previousTarget이 null 이면 첫 번째 반환
        if (previousTarget == null)
            return targets[0];

        int idx = targets.IndexOf(previousTarget);
        // 만약 리스트에 없으면 첫 번째
        if (idx < 0)
            return targets[0];

        // 다음 인덱스로 순환
        int next = (idx + 1) % targets.Count;
        return targets[next];
    }

    void OnDisable()
    {
        if (pathRoutine != null)
            StopCoroutine(pathRoutine);
    }

    private void StickToNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
            transform.position = hit.position;
    }
}
