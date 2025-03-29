using UnityEngine;
using System.Collections;

public class RulerController : MonoBehaviour
{
    [Header("참조 설정")]
    public Transform startPoint;    // 시작점
    public Transform endPoint;      // 실제 끝점
    public Rigidbody2D rb;

    [Header("라인 길이 조절")]
    public float maxLineLength = 5f;  // 최대 허용 길이
    public float retractSpeed = 5f;   // 선이 줄어드는 속도

    [Header("변수 조정")]
    public float forcePower = 5f;     // 선이 줄어들 때 가해지는 힘
    private bool isConnected = false;
    private LineRenderer lineRenderer;
    // 코루틴에서 업데이트 되는 현재 선의 끝점
    private Vector3 currentEndPosition;
    private Coroutine coroutine;

    void OnEnable()
    {
        isConnected = true;
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        if (endPoint != null)
        {
            currentEndPosition = endPoint.position;
        }
    }

    void Update()
    {
        if (startPoint == null || endPoint == null || !lineRenderer.enabled)
            return;

        Vector3 startPos = startPoint.position;
        // 코루틴이 실행 중이면 currentEndPosition을 사용하고,
        // 그렇지 않으면 endPoint.position을 사용합니다.
        Vector3 displayEndPos = (coroutine != null) ? currentEndPosition : endPoint.position;

        // 시작점과 실제 끝점 사이의 거리는 endPoint.position으로 계산합니다.
        float distance = Vector3.Distance(startPos, endPoint.position);
        // LineRenderer에 시작점과 표시할 끝점을 설정합니다.
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, displayEndPos);
        if (distance > maxLineLength)
        {
            this.enabled = false;
        }
    }
    public void OnDisable()
    {
        ForceBack(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
        isConnected = false;
        lineRenderer.enabled = false;
    }
    private void ForceBack(Vector2 start, Vector2 end)
    {
        Debug.Log("충격 발생 " + this.gameObject.name + " " + (start - end).normalized);
        rb.AddForce((start - end).normalized * forcePower * Vector2.Distance(start, end), ForceMode2D.Impulse);
    }
}
