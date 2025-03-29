using UnityEngine;
using System.Collections;

public class RulerController : MonoBehaviour
{
    [Header("잡기 설정")]
    public KeyCode grabKey = KeyCode.Space;      // 인스펙터에서 지정할 잡기 키
    public Transform handle;                     // 손잡이 위치
    public float grabDistance = 1.5f;            // 손잡이 앞 감지 거리
    public LayerMask grabMask;                   // 잡을 수 있는 레이어 지정

    [Header("줄 설정")]
    public float maxLineLength = 5f;
    public float retractSpeed = 5f;
    public float forcePower = 5f;

    [Header("참조")]
    public LineRenderer lineRenderer;
    public Rigidbody2D myRb;

    private Transform target;                    // 현재 잡은 대상
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
                // 내 리지드바디는 멈추고, 상대 Handle은 이동하게 함
                myRb.velocity = Vector2.zero;
                myRb.bodyType = RigidbodyType2D.Static;
                targetRb.bodyType = RigidbodyType2D.Dynamic;
            }

            UpdateLine(); // 시작하자마자 줄 그림
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

        // 탄성 효과 적용
        if (retractCoroutine != null) StopCoroutine(retractCoroutine);
        retractCoroutine = StartCoroutine(ApplyElasticForce());

        // 내 리지드바디 다시 활성화
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

        // 진짜 물리적인 반동
        if (targetRb)
        {
            Vector2 forceDir = (handle.position - end).normalized;
            float dist = Vector2.Distance(handle.position, end);
            targetRb.AddForce(forceDir * dist * forcePower, ForceMode2D.Impulse);
        }
    }
}
