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

    void Update()
    {
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

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!Input.GetKeyDown(grabKey)) return;
        if (!collision.CompareTag("Box") && !collision.CompareTag("Handle")) return;

        Debug.Log($"Trigger detected with: {collision.name}");

        if (collision.CompareTag("Box"))
        {
            grabbedObject = collision.transform;
            grabbedObject.SetParent(handle);
            grabbedObject.localPosition = Vector3.zero;
            isGrabbing = true;
        }
        else
        {
            grabbedObject = collision.transform;
            handle.SetParent(grabbedObject);
            isGrabbing = true;

            if (lineRenderer != null) lineRenderer.enabled = true;
            if (lineCollider != null) lineCollider.enabled = true;

            UpdateLine();
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
                lineCollider.transform.InverseTransformPoint(p1),
                lineCollider.transform.InverseTransformPoint(p2)
            };
        }

        Debug.DrawLine(p1, p2, Color.white);
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

        // 플레이어 반동
        Rigidbody2D playerRb = originalParent.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.AddForce(playerKnockbackDirection.normalized * stretchLength * playerKnockbackPower, ForceMode2D.Impulse);
        }

        grabbedObject = null;
        stretchLength = 0f;
    }

    IEnumerator SmoothHandleReturn()
    {
        Transform player = originalParent; // 당길 대상은 플레이어 본체
        Vector3 start = player.position;
        Vector3 end = handle.position;

        // stretchLength에 따라 당기는 속도 계산
        float duration = Mathf.Clamp(0.1f + stretchLength * 0.05f, 0.1f, 0.5f);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            player.position = Vector3.Lerp(start, end, t);
            UpdateLine(); // 줄 같이 줄이기
            yield return null;
        }

        player.position = end;

        // 줄 시각적 제거
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (lineCollider != null) lineCollider.enabled = false;

        // 핸들도 위치 보정
        handle.localPosition = originalLocalPos;

        Debug.Log("Player pulled back smoothly.");
    }

}
