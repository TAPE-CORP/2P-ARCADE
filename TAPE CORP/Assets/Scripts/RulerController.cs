using UnityEngine;
using System.Collections;

public class RulerController : MonoBehaviour
{
    [Header("핸들 및 조작")]
    public Transform handle;
    public KeyCode grabKey = KeyCode.Space;
    public LayerMask grabMask;

    [Header("줄자 효과 (Line 오브젝트 내에 있음)")]
    public Transform lineObject;
    public LineRenderer lineRenderer;
    public EdgeCollider2D lineCollider;

    [Header("반동 설정")]
    public float returnForcePower = 10f;
    public Vector2 playerKnockbackDirection = Vector2.left;
    public float playerKnockbackPower = 5f;
    public float pullDuration = 0.25f;

    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Transform grabbedObject;
    public bool isGrabbing = false;

    [HideInInspector] public float stretchLength = 0f;

    void Start()
    {
        originalParent = handle.parent;
        originalLocalPos = handle.localPosition;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.useWorldSpace = true;
        }

        if (lineCollider != null)
        {
            lineCollider.enabled = false;
        }

        Debug.Log("RulerController initialized.");
    }
    void UpdateLine()
    {
        Vector3 p1 = handle.position;
        Vector3 p2 = originalParent.position;

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, p1);
            lineRenderer.SetPosition(1, p2);
        }

        if (lineCollider != null)
        {
            lineCollider.points = new Vector2[]
            {
            lineObject.InverseTransformPoint(p1),
            lineObject.InverseTransformPoint(p2)
            };
        }
    }
    void Update()
    {
        if (!isGrabbing && Input.GetKeyDown(grabKey))
        {
            TryGrabNearby();
        }

        if (isGrabbing)
        {
            UpdateLine();
            stretchLength = Vector2.Distance(handle.position, originalParent.position);

            if (!Input.GetKey(grabKey))
            {
                Debug.Log("Grab released.");
                Release();
            }
        }
    }

    void TryGrabNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(handle.position, 1.5f, grabMask);
        Debug.Log($"[Grab Attempt] Found {hits.Length} colliders nearby.");

        foreach (var hit in hits)
        {
            Debug.Log($"Checking {hit.name} with tag {hit.tag}");

            if (!hit.CompareTag("Box") && !hit.CompareTag("Handle")) continue;

            grabbedObject = FindParentWithNameContains(hit.transform, "player");
            if (grabbedObject == null)
            {
                Debug.LogWarning("No player root found!");
                continue;
            }

            handle.SetParent(grabbedObject);
            isGrabbing = true;

            if (lineRenderer != null) lineRenderer.enabled = true;
            if (lineCollider != null) lineCollider.enabled = true;

            UpdateLine();
            break;
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (!Input.GetKey(grabKey)) return;

        if (!collision.CompareTag("Box") && !collision.CompareTag("Handle")) return;

        // 거리가 너무 멀면 무시
        float dist = Vector2.Distance(handle.position, collision.transform.position);
        if (dist > 1.5f) return;

        Debug.Log($"Trigger detected with: {collision.name}");

        if (collision.CompareTag("Box"))
        {
            grabbedObject = collision.transform;
            grabbedObject.SetParent(handle);
            grabbedObject.localPosition = Vector3.zero;
            isGrabbing = true;

            if (lineRenderer != null) lineRenderer.enabled = true;
            if (lineCollider != null) lineCollider.enabled = true;

            UpdateLine(); // 첫 줄 생성 확실히 보장
        }
        else if (collision.CompareTag("Handle"))
        {
            grabbedObject = collision.transform;
            handle.SetParent(grabbedObject);
            isGrabbing = true;

            if (lineRenderer != null) lineRenderer.enabled = true;
            if (lineCollider != null) lineCollider.enabled = true;

            UpdateLine(); // 줄 좌표 즉시 갱신
        }
    }

    void Release()
    {
        isGrabbing = false;

        if (lineRenderer != null) lineRenderer.enabled = true; // 줄 유지
        if (lineCollider != null) lineCollider.enabled = true;

        // 핸들 부모만 복귀 (위치는 천천히 이동)
        handle.SetParent(originalParent);
        StartCoroutine(SmoothHandleReturn());

        if (grabbedObject != null)
        {
            Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();
            Vector3 pullTarget = originalParent.position;

            if (rb != null)
            {
                Vector2 dir = (pullTarget - grabbedObject.position).normalized;
                float dist = Vector2.Distance(pullTarget, grabbedObject.position);
                rb.AddForce(dir * dist * returnForcePower, ForceMode2D.Impulse);
                Debug.Log($"Rebound force applied: {dir * dist * returnForcePower}");
            }

            if (grabbedObject.parent == handle)
            {
                grabbedObject.SetParent(null);
                Debug.Log("Box was attached to handle, now detached.");
            }
        }

        stretchLength = 0f;
    }
    IEnumerator SmoothHandleReturn()
    {
        if (grabbedObject == null) yield break;

        Transform toPull = FindParentWithNameContains(grabbedObject, "player");
        if (toPull == null) yield break;

        Vector3 start = toPull.position;
        Vector3 end = originalParent.position;

        float duration = Mathf.Clamp(0.1f + stretchLength * 0.05f, 0.1f, 0.5f);
        float t = 0f;

        Rigidbody2D pulledRb = toPull.GetComponent<Rigidbody2D>();
        Rigidbody2D myRb = originalParent.GetComponent<Rigidbody2D>();

        if (pulledRb != null)
        {
            pulledRb.velocity = Vector2.zero;
            pulledRb.isKinematic = true;
        }

        if (myRb != null)
        {
            myRb.velocity = Vector2.zero;
        }

        float lineShrinkFactor = 1.5f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float lineT = Mathf.Clamp01(t * lineShrinkFactor);

            Vector3 pullPos = Vector3.Lerp(start, end, t);
            Vector3 fakeHandlePos = Vector3.Lerp(start, end, lineT);

            toPull.position = pullPos;
            handle.position = fakeHandlePos;

            UpdateLine();

            yield return null;
        }

        // 도착 후 물리 다시 활성화
        if (pulledRb != null)
            pulledRb.isKinematic = false;

        handle.localPosition = originalLocalPos;
        handle.SetParent(originalParent);

        if (lineRenderer != null) lineRenderer.enabled = false;
        if (lineCollider != null) lineCollider.enabled = false;

        if (myRb != null)
        {
            Vector2 pullDir = (originalParent.position - toPull.position).normalized;

            // 위로 튕기는 방향 추가
            Vector2 launchDir = (pullDir + Vector2.up).normalized;

            float knockPower = Mathf.Clamp(stretchLength, 1f, 10f);

            //  강한 물리 반작용
            myRb.AddForce(launchDir * knockPower * playerKnockbackPower, ForceMode2D.Impulse);

  
            Debug.Log($"[반작용 force] {launchDir * knockPower * playerKnockbackPower}");
        }
        Debug.Log("Entire player pulled to original position.");

        grabbedObject = null;
    }

    Transform FindParentWithNameContains(Transform child, string keyword)
    {
        Transform current = child;
        while (current != null)
        {
            if (current.name.Contains(keyword))
                return current;
            current = current.parent;
        }
        return null;
    }
}
