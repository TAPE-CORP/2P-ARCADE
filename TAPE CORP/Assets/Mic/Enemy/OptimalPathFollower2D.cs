using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class OptimalPathFollower2D : MonoBehaviour
{
    [Header("���� ��� (2������)")]
    [Tooltip("������� �Ҵ��ϼ���. �ϳ��� ���󰩴ϴ�.")]
    public List<Transform> targets = new List<Transform>(2);

    [Header("�̵� �ӵ�")]
    public float moveSpeed = 4f;

    [Header("��� ���� ���� (��)")]
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
            Debug.LogError("������ Ÿ���� �����ϴ�.");
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
            // ���� Ÿ�� ��û
            currentTarget = PickNextTarget(currentTarget);
            corners.Clear();
        }
    }

    /// <summary>
    /// ������ ������ previousTarget�� �����ϰ�, ����Ʈ���� ����ִ�(== null �ƴ�) ù Ÿ���� ��ȯ�մϴ�.
    /// </summary>
    private Transform PickNextTarget(Transform previousTarget)
    {
        // ����Ʈ ����: null ���� ����
        targets.RemoveAll(t => t == null);
        if (targets.Count == 0)
            return null;

        // previousTarget�� null �̸� ù ��° ��ȯ
        if (previousTarget == null)
            return targets[0];

        int idx = targets.IndexOf(previousTarget);
        // ���� ����Ʈ�� ������ ù ��°
        if (idx < 0)
            return targets[0];

        // ���� �ε����� ��ȯ
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
