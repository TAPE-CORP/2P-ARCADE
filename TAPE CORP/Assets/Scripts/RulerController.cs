using UnityEngine;
using System.Collections;
using TMPro;

public class RulerController : MonoBehaviour
{
    [Header("�ڵ� �� ����")]
    public Transform handle;
    public KeyCode grabKey = KeyCode.Space;
    public LayerMask grabMask;

    [Header("TMP �ؽ�Ʈ ����")]
    public TMP_Text stretchLengthText; //  ���� ���� ǥ�� �ؽ�Ʈ

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
        Gizmos.DrawWireSphere(handle.position, 3f); // grabRange�� ����
    }
    void TryGrabNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(handle.position, 1.5f, grabMask);
        Debug.Log($"[Grab Attempt] Found {hits.Length} colliders nearby.");

        foreach (var hit in hits)
        {
            Debug.Log($"���ƾӾ� {hit.name} with tag {hit.tag}");

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

                //  �ڽ��� ���� ���� ����
                Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.isKinematic = true;

                isGrabbing = true;

                // �� ����
                lineRenderer?.SetPositions(new Vector3[2]);
                lineRenderer.enabled = false;
                lineCollider.enabled = false;
            }
            else // Player �Ǵ� Handle�� ���
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

            break; // ù ��� ��� ����
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!Input.GetKey(grabKey)) return;

        if (!collision.CompareTag("Box") && !collision.CompareTag("Handle")) return;

        // �Ÿ��� �ʹ� �ָ� ����
        float dist = Vector2.Distance(handle.position, collision.transform.position);
        if (dist > 1.5f) return;

        Debug.Log($"�ִٰ� ���ڽľ�: {collision.name}");

        if (collision.CompareTag("Box"))
        {
            grabbedObject = collision.transform;
            grabbedObject.SetParent(handle);

            // ���̱� ��ġ ����
            float boxHalfWidth = grabbedObject.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f;
            grabbedObject.localPosition = new Vector3(boxHalfWidth + 0.2f, 0f, 0f);

            isGrabbing = true;

            if (lineRenderer != null) lineRenderer.enabled = true;
            if (lineCollider != null) lineCollider.enabled = true;

            UpdateLine();
        }
        else if (collision.CompareTag("Handle"))
        {
            Debug.Log($"��ƶ�!!!!!!!!!!!!!!!!! {collision.name}");
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

        // �ڵ� ������� ����
        handle.SetParent(originalParent);
        StartCoroutine(SmoothHandleReturn());

        if (grabbedObject != null)
        {
            Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();

            //  Box�� ���: �θ� ���� ���� �ٽ� �ѱ�
            if (grabbedObject.parent == handle)
            {
                grabbedObject.SetParent(null);

                if (rb != null)
                    rb.isKinematic = false;

                Debug.Log("Box was attached to handle, now detached.");
            }
            else // Player�� ���: �ݵ� ó��
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

        // ���� �� ���� �ٽ� Ȱ��ȭ
        if (pulledRb != null)
            pulledRb.isKinematic = false;

        handle.localPosition = originalLocalPos;
        handle.SetParent(originalParent);

        if (lineRenderer != null) lineRenderer.enabled = false;
        if (lineCollider != null) lineCollider.enabled = false;

        if (myRb != null)
        {
            Vector2 pullDir = (originalParent.position - toPull.position).normalized;

            // ���� ƨ��� ���� �߰�
            Vector2 launchDir = (pullDir + Vector2.up).normalized;

            float knockPower = Mathf.Clamp(stretchLength, 1f, 10f);

            //  ���� ���� ���ۿ�
            myRb.AddForce(launchDir * knockPower * playerKnockbackPower, ForceMode2D.Impulse);

  
            Debug.Log($"[���ۿ� force] {launchDir * knockPower * playerKnockbackPower}");
        }
        Debug.Log("Entire player pulled to original position.");

        grabbedObject = null;
    }

    Transform FindParentWithNameContains(Transform child, string keyword)
    {
        Transform current = child;
        while (current != null)
        {
            if (current.name.ToLower().Contains(keyword.ToLower())) // ��ҹ��� ����
                return current;
            current = current.parent;
        }
        return null;
    }
}
