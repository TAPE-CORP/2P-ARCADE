// Conveyor.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Conveyor : MonoBehaviour
{
    [Tooltip("�����̾� ��Ʈ�� �ӵ�")]
    public float speed = 1.5f;
    [Tooltip("�����̾� ���� ���� �÷��� (ȸ�� ��ۿ�)")]
    public bool isRight = true;

    private Vector2 moveDirection;
    private float _originalZRotation;

    void Awake()
    {
        // ���� Z ȸ�� ����
        _originalZRotation = transform.localEulerAngles.z;
        ApplyFlip();

        // Ʈ���ŷ� ����
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        // �׻� transform.right ���� �״�� ���
        var dir3 = transform.right.normalized;
        moveDirection = new Vector2(dir3.x, dir3.y);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Translate ������� �о��ֱ� (�÷��̾� �̵��� �ڿ������� �ջ��)
        collision.transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    public void SetDirection()
    {
        // �÷��� ��� �� 180�� ȸ��
        isRight = !isRight;
        ApplyFlip();
    }

    private void ApplyFlip()
    {
        float targetZ = isRight
            ? _originalZRotation
            : _originalZRotation + 180f;
        transform.localEulerAngles = new Vector3(0, 0, targetZ);
    }
}
