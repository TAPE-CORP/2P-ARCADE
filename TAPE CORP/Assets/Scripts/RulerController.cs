using UnityEngine;
using System.Collections;

public class RulerController : MonoBehaviour
{
    [Header("�ڵ� �� ����")]
    public Transform handle;
    public KeyCode grabKey = KeyCode.Space;
    public LayerMask grabMask;

    [Header("���� ȿ�� (Line ������Ʈ ���� ����)")]
    public Transform lineObject;
    public LineRenderer lineRenderer;
    public EdgeCollider2D lineCollider;

    [Header("�ݵ� ����")]
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
        if (isGrabbing)
        {
            Debug.Log("Updating Line...");
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
        if (!Input.GetKey(grabKey)) return;

        if (!collision.CompareTag("Box") && !collision.CompareTag("Handle")) return;

        // �Ÿ��� �ʹ� �ָ� ����
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

            UpdateLine(); // ù �� ���� Ȯ���� ����
        }
        else if (collision.CompareTag("Handle"))
        {
            grabbedObject = collision.transform;
            handle.SetParent(grabbedObject);
            isGrabbing = true;

            if (lineRenderer != null) lineRenderer.enabled = true;
            if (lineCollider != null) lineCollider.enabled = true;

            UpdateLine(); // �� ��ǥ ��� ����
        }
    }

    void Release()
    {
        isGrabbing = false;

        if (lineRenderer != null) lineRenderer.enabled = true; // �� ����
        if (lineCollider != null) lineCollider.enabled = true;

        // �ڵ� �θ� ���� (��ġ�� õõ�� �̵�)
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

        // �÷��̾� �ݵ�
        Rigidbody2D playerRb = originalParent.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.AddForce(playerKnockbackDirection.normalized * stretchLength * playerKnockbackPower, ForceMode2D.Impulse);
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

        Rigidbody2D rb = toPull.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // ���� �� ���� �پ�鵵�� �ӵ� ��� �ο�
        float lineShrinkFactor = 1.5f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // �� ��ġ�� �÷��̾�� �� ������ �پ�� ��ó�� ���̰�
            float lineT = Mathf.Clamp01(t * lineShrinkFactor);

            Vector3 pullPos = Vector3.Lerp(start, end, t);
            Vector3 fakeHandlePos = Vector3.Lerp(start, end, lineT);

            toPull.position = pullPos;
            handle.position = fakeHandlePos; // handle ��ġ�� �׻� �� ���� ���� ����ȭ

            UpdateLine(); // �ٵ� ���� ���̱�
            yield return null;
        }

        toPull.position = end;
        handle.localPosition = originalLocalPos;
        handle.SetParent(originalParent);

        if (rb != null)
            rb.isKinematic = false;

        if (lineRenderer != null) lineRenderer.enabled = false;
        if (lineCollider != null) lineCollider.enabled = false;

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
