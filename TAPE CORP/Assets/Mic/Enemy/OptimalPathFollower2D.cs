using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
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
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
            transform.position = hit.position;
        else
            Debug.LogWarning("NavMesh 위에 시작점을 찾지 못했습니다.");

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
            NavMeshHit startHit, endHit;
            bool hasStart = NavMesh.SamplePosition(transform.position, out startHit, 1f, NavMesh.AllAreas);
            bool hasEnd = NavMesh.SamplePosition(target.position, out endHit, 1f, NavMesh.AllAreas);
            if (hasStart && hasEnd && NavMesh.CalculatePath(startHit.position, endHit.position, NavMesh.AllAreas, navPath))
                UpdateCorners();
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
        if (corners.Count == 0 || currentCorner >= corners.Count)
            return;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = corners[currentCorner];
        Vector2 direction = (targetPos - currentPos).normalized;

        // 오른쪽이 정면이므로 right 사용
        if (direction.sqrMagnitude > 0.0001f)
            transform.right = direction;

        Vector2 nextPos = currentPos + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);

        if (Vector2.Distance(currentPos, targetPos) < 0.1f)
            currentCorner++;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == target.gameObject)
        {
            Destroy(target.gameObject);
            Destroy(gameObject);
        }
    }
}