using UnityEngine;
using System.Collections;

public class RulerController : MonoBehaviour
{
    [Header("��� ����")]
    public KeyCode grabKey = KeyCode.Space;      // �ν����Ϳ��� ������ ��� Ű
    public Transform handle;                     // ������ ��ġ
    public float grabDistance = 1.5f;            // ������ �� ���� �Ÿ�
    public LayerMask grabMask;                   // ���� �� �ִ� ���̾� ����

    [Header("�� ����")]
    public float maxLineLength = 5f;
    public float retractSpeed = 5f;
    public float forcePower = 5f;

    [Header("����")]
    public LineRenderer lineRenderer;
    public Rigidbody2D myRb;

    private Transform target;                    // ���� ���� ���
    private Rigidbody2D targetRb;
    private Vector3 currentEndPosition;
    private Coroutine retractCoroutine;
    private bool isGrabbing = false;

    private void Start()
    {
        if (!myRb) myRb = GetComponent<Rigidbody2D>();
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(grabKey))
        {
            TryGrab();
        }
        else if (Input.GetKey(grabKey) && isGrabbing)
        {
            UpdateLine();
            if (Vector2.Distance(handle.position, target.position) > maxLineLength)
            {
                ReleaseGrab();
            }
        }
        else if (Input.GetKeyUp(grabKey) && isGrabbing)
        {
            ReleaseGrab();
        }
    }

    private void TryGrab()
    {
        RaycastHit2D hit = Physics2D.Raycast(handle.position, transform.right, grabDistance, grabMask);
        if (hit.collider != null && (hit.collider.CompareTag("Box") || hit.collider.CompareTag("Handle")))
        {
            target = hit.collider.transform;
            targetRb = target.GetComponent<Rigidbody2D>();
            isGrabbing = true;
            lineRenderer.enabled = true;
            currentEndPosition = target.position;

            if (target.CompareTag("Handle"))
            {
                // �� ������ٵ�� ���߰�, ��� Handle�� �̵��ϰ� ��
                myRb.velocity = Vector2.zero;
                myRb.bodyType = RigidbodyType2D.Static;
                targetRb.bodyType = RigidbodyType2D.Dynamic;
            }

            UpdateLine(); // �������ڸ��� �� �׸�
        }
    }

    private void UpdateLine()
    {
        if (!target || !isGrabbing) return;

        Vector3 start = handle.position;
        Vector3 end = target.position;
        currentEndPosition = end;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void ReleaseGrab()
    {
        if (!target) return;

        isGrabbing = false;
        lineRenderer.enabled = false;

        // ź�� ȿ�� ����
        if (retractCoroutine != null) StopCoroutine(retractCoroutine);
        retractCoroutine = StartCoroutine(ApplyElasticForce());

        // �� ������ٵ� �ٽ� Ȱ��ȭ
        myRb.bodyType = RigidbodyType2D.Dynamic;
        target = null;
        targetRb = null;
    }

    private IEnumerator ApplyElasticForce()
    {
        Vector3 start = handle.position;
        Vector3 end = currentEndPosition;

        float t = 0f;
        while (t < 1f)
        {
            currentEndPosition = Vector3.Lerp(end, start, t);
            t += Time.deltaTime * retractSpeed;

            if (lineRenderer.enabled)
            {
                lineRenderer.SetPosition(1, currentEndPosition);
            }

            yield return null;
        }

        // ��¥ �������� �ݵ�
        if (targetRb)
        {
            Vector2 forceDir = (handle.position - end).normalized;
            float dist = Vector2.Distance(handle.position, end);
            targetRb.AddForce(forceDir * dist * forcePower, ForceMode2D.Impulse);
        }
    }
}
