using UnityEngine;
using System.Collections;
using TMPro;

public class RulerController : MonoBehaviour
{
    [Header("핸들 및 조작")]
    public Transform handle;
    public KeyCode grabKey = KeyCode.Space;
    public LayerMask grabMask;

    [Header("TMP 텍스트 설정")]
    public TMP_Text stretchLengthText; //  줄자 길이 표시 텍스트

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

        if (stretchLengthText != null)
            stretchLengthText.text = "0.0m";
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

            if (!Input.GetKey(grabKey))
            {
                Release();
            }
        }
        else
        {
            if (stretchLengthText != null)
                stretchLengthText.text = "0.0m";
        }
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

        stretchLength = Vector2.Distance(p1, p2);

        if (stretchLengthText != null)
        {
            stretchLengthText.text = stretchLength.ToString("F2") + "m";

            Vector3 parentLossy = stretchLengthText.transform.parent.lossyScale;
            Vector3 localScale = stretchLengthText.transform.localScale;
            localScale.x = (parentLossy.x < 0) ? -1f : 1f;
            stretchLengthText.transform.localScale = localScale;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(handle.position, 3f); // grabRange와 같게
    }
    void TryGrabNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(handle.position, 1.5f, grabMask);
        Debug.Log($"[Grab Attempt] Found {hits.Length} colliders nearby.");

        foreach (var hit in hits)
        {
            Debug.Log($"으아앙악 {hit.name} with tag {hit.tag}");

            if (!hit.CompareTag("Box") && !hit.CompareTag("Handle") && !hit.CompareTag("Player")) continue;

            if (hit.CompareTag("Box"))
            {
                grabbedObject = FindParentWithNameContains(hit.transform, "box");
                if (grabbedObject == null)
                {
                    Debug.LogWarning("No box root found!");
                    continue;
                }

                grabbedObject.SetParent(handle);

                float boxHalfWidth = grabbedObject.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f;
                grabbedObject.localPosition = new Vector3(boxHalfWidth + 0.2f, 0f, 0f);

                //  박스의 물리 제어 끄기
                Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.isKinematic = true;

                isGrabbing = true;

                // 줄 없음
                lineRenderer?.SetPositions(new Vector3[2]);
                lineRenderer.enabled = false;
                lineCollider.enabled = false;
            }
            else // Player 또는 Handle일 경우
            {
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
            }

            break; // 첫 대상만 잡고 종료
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!Input.GetKey(grabKey)) return;

        if (!collision.CompareTag("Box") && !collision.CompareTag("Handle")) return;

        // 거리가 너무 멀면 무시
        float dist = Vector2.Distance(handle.position, collision.transform.position);
        if (dist > 1.5f) return;

        Debug.Log($"멀다고 이자식아: {collision.name}");

        if (collision.CompareTag("Box"))
        {
            grabbedObject = collision.transform;
            grabbedObject.SetParent(handle);

            // 붙이기 위치 조정
            float boxHalfWidth = grabbedObject.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f;
            grabbedObject.localPosition = new Vector3(boxHalfWidth + 0.2f, 0f, 0f);

            isGrabbing = true;

            if (lineRenderer != null) lineRenderer.enabled = true;
            if (lineCollider != null) lineCollider.enabled = true;

            UpdateLine();
        }
        else if (collision.CompareTag("Handle"))
        {
            Debug.Log($"잡아라!!!!!!!!!!!!!!!!! {collision.name}");
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

        // 핸들 원래대로 복귀
        handle.SetParent(originalParent);
        StartCoroutine(SmoothHandleReturn());

        if (grabbedObject != null)
        {
            Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();

            //  Box일 경우: 부모 끊고 물리 다시 켜기
            if (grabbedObject.parent == handle)
            {
                grabbedObject.SetParent(null);

                if (rb != null)
                    rb.isKinematic = false;

                Debug.Log("Box was attached to handle, now detached.");
            }
            else // Player일 경우: 반동 처리
            {
                Vector3 pullTarget = originalParent.position;

                if (rb != null)
                {
                    Vector2 dir = (pullTarget - grabbedObject.position).normalized;
                    float dist = Vector2.Distance(pullTarget, grabbedObject.position);
                    rb.AddForce(dir * dist * returnForcePower, ForceMode2D.Impulse);
                    Debug.Log($"Rebound force applied: {dir * dist * returnForcePower}");
                }
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
            if (current.name.ToLower().Contains(keyword.ToLower())) // 대소문자 무시
                return current;
            current = current.parent;
        }
        return null;
    }
}
