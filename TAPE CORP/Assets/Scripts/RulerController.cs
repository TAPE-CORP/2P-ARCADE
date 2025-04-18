using UnityEngine;
using System.Collections;
using TMPro;

public class RulerController : MonoBehaviour
{
    [Header("핸들 및 조작")]
    public Transform handle;
    public KeyCode grabKey = KeyCode.Space;
    public LayerMask grabMask;
    public float tryDis = 3f;

    [Header("TMP 텍스트 설정")]
    public TMP_Text stretchLengthText;

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
        Gizmos.DrawWireSphere(handle.position, 3f);
    }

    void TryGrabNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(handle.position, tryDis, grabMask);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Box") && !hit.CompareTag("Obj") && !hit.CompareTag("Handle") && !hit.CompareTag("Player")) continue;

            if (hit.CompareTag("Box") || hit.CompareTag("Obj"))
            {
                grabbedObject = FindParentWithNameContains(hit.transform, "box", "obj");
                if (grabbedObject == null) continue;

                grabbedObject.SetParent(handle);
                SpriteRenderer sr = grabbedObject.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    float objectWidth = sr.bounds.size.x;
                    float handleWidth = handle.GetComponent<SpriteRenderer>()?.bounds.size.x ?? 0f;
                    float offsetX = (objectWidth + handleWidth) * 0.5f + 0.015f;
                    grabbedObject.localPosition = new Vector3(offsetX, 0f, 0f);
                    grabbedObject.localEulerAngles = Vector3.zero;
                    grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                }
                else
                {
                    grabbedObject.localPosition = new Vector3(0.5f, 0f, 0f); // fallback
                }

                Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();
                if (rb != null) rb.isKinematic = true;

                isGrabbing = true;
                lineRenderer?.SetPositions(new Vector3[2]);
                lineRenderer.enabled = false;
                lineCollider.enabled = false;
            }
            else
            {
                grabbedObject = FindParentWithNameContains(hit.transform, "player");
                if (grabbedObject == null) continue;

                handle.SetParent(grabbedObject);
                isGrabbing = true;

                if (lineRenderer != null) lineRenderer.enabled = true;
                if (lineCollider != null) lineCollider.enabled = true;

                UpdateLine();
            }
            break;
        }
    }

    void Release()
    {
        isGrabbing = false;

        if (lineRenderer != null) lineRenderer.enabled = true;
        if (lineCollider != null) lineCollider.enabled = true;

        handle.SetParent(originalParent);
        StartCoroutine(SmoothHandleReturn());

        if (grabbedObject != null)
        {
            Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();

            if (grabbedObject.parent == handle)
            {
                grabbedObject.SetParent(null);
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
            else
            {
                int grabbedLayer = grabbedObject.gameObject.layer;
                int boxLayer = LayerMask.NameToLayer("Box");
                int objLayer = LayerMask.NameToLayer("Obj");

                if (grabbedLayer != boxLayer && grabbedLayer != objLayer)
                {
                    Vector3 pullTarget = originalParent.position;

                    if (rb != null)
                    {
                        Vector2 dir = (pullTarget - grabbedObject.position).normalized;
                        float dist = Vector2.Distance(pullTarget, grabbedObject.position);
                        rb.AddForce(dir * dist * returnForcePower, ForceMode2D.Impulse);
                    }
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

        if (pulledRb != null) pulledRb.isKinematic = false;

        handle.localPosition = originalLocalPos;
        handle.SetParent(originalParent);

        if (lineRenderer != null) lineRenderer.enabled = false;
        if (lineCollider != null) lineCollider.enabled = false;

        if (myRb != null && lineRenderer != null)
        {
            Vector3 startPos = lineRenderer.GetPosition(0);
            Vector3 endPos = lineRenderer.GetPosition(1);
            Vector3 forceDir = (startPos - endPos).normalized;
            float knockPower = Mathf.Clamp(stretchLength, 1f, 10f);
            myRb.AddForce(forceDir * knockPower * playerKnockbackPower, ForceMode2D.Impulse);
        }

        grabbedObject = null;
    }

    Transform FindParentWithNameContains(Transform child, string keywordA, string keywordB = null)
    {
        Transform current = child;
        while (current != null)
        {
            string lowerName = current.name.ToLower();
            if (lowerName.Contains(keywordA.ToLower()) ||
                (keywordB != null && lowerName.Contains(keywordB.ToLower())))
                return current;
            current = current.parent;
        }
        return null;
    }
}