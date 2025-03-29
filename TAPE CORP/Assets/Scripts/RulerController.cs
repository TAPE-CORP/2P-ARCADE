using UnityEngine;

public class RulerController : MonoBehaviour
{
    [Header("핸들 및 조작")]
    public Transform handle;
    public KeyCode grabKey = KeyCode.Space;
    public LayerMask grabMask;

    [Header("줄자 효과 (Line 오브젝트 내에 있음)")]
    public Transform lineObject;                 // "line" 오브젝트
    public LineRenderer lineRenderer;
    public EdgeCollider2D lineCollider;

    [Header("반동 설정")]
    public float returnForcePower = 10f;
    public Vector2 playerKnockbackDirection = Vector2.left;
    public float playerKnockbackPower = 5f;

    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Transform grabbedObject;
    public bool isGrabbing = false;
    private float directionSign = 1f;

    [HideInInspector] public float stretchLength = 0f;

    void Start()
    {
        originalParent = handle.parent;
        originalLocalPos = handle.localPosition;
        directionSign = Mathf.Sign(originalParent.localScale.x);

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

        if (lineRenderer != null) lineRenderer.enabled = false;
        if (lineCollider != null) lineCollider.enabled = false;

        handle.SetParent(originalParent);
        handle.localPosition = originalLocalPos; // 방향 무시하고 원래 위치로 복귀

        Debug.Log("Handle returned to original parent.");

        if (grabbedObject != null)
        {
            Rigidbody2D rb = grabbedObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (originalParent.position - grabbedObject.position).normalized;
                rb.AddForce(dir * stretchLength * returnForcePower, ForceMode2D.Impulse);
                Debug.Log("Rebound force applied to grabbed object.");
            }

            if (grabbedObject.parent == handle)
            {
                grabbedObject.SetParent(null);
                Debug.Log("Box was attached to handle, now detached.");
            }
        }

        Rigidbody2D playerRb = originalParent.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.AddForce(playerKnockbackDirection.normalized * stretchLength * playerKnockbackPower, ForceMode2D.Impulse);
            Debug.Log("Knockback applied to player.");
        }

        grabbedObject = null;
        stretchLength = 0f;
    }


}
