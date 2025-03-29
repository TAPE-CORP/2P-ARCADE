using UnityEngine;
using System.Collections;

public class RulerController : MonoBehaviour
{
    [Header("���� ����")]
    public Transform startPoint;    // ������
    public Transform endPoint;      // ���� ����
    public Rigidbody2D rb;

    [Header("���� ���� ����")]
    public float maxLineLength = 5f;  // �ִ� ��� ����
    public float retractSpeed = 5f;   // ���� �پ��� �ӵ�

    [Header("���� ����")]
    public float forcePower = 5f;     // ���� �پ�� �� �������� ��
    private bool isConnected = false;
    private LineRenderer lineRenderer;
    // �ڷ�ƾ���� ������Ʈ �Ǵ� ���� ���� ����
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
        // �ڷ�ƾ�� ���� ���̸� currentEndPosition�� ����ϰ�,
        // �׷��� ������ endPoint.position�� ����մϴ�.
        Vector3 displayEndPos = (coroutine != null) ? currentEndPosition : endPoint.position;

        // �������� ���� ���� ������ �Ÿ��� endPoint.position���� ����մϴ�.
        float distance = Vector3.Distance(startPos, endPoint.position);
        // LineRenderer�� �������� ǥ���� ������ �����մϴ�.
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
        Debug.Log("��� �߻� " + this.gameObject.name + " " + (start - end).normalized);
        rb.AddForce((start - end).normalized * forcePower * Vector2.Distance(start, end), ForceMode2D.Impulse);
    }
}
