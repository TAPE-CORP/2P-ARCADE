using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 1.5f; // �����̾� ��Ʈ�� �ӵ�
    public bool isRight = true;

    private Vector2 moveDirection;
    private Vector3 _originalScale;

    void Awake()
    {
        // ���� ������ ����
        _originalScale = transform.localScale;
        ApplyFlip();
    }

    void Update()
    {
        // ���� ���� (��ũ�� ����� ��ġ)
        moveDirection = isRight ? Vector2.right : Vector2.left;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

        if (rb != null && (rb.gameObject.CompareTag("Holder") || rb.gameObject.CompareTag("Box")))
        {
            rb.gravityScale = 0;
            rb.velocity = moveDirection * speed;
        }
        else if (rb != null && collision.gameObject.layer != LayerMask.NameToLayer("Map"))
        {
            collision.transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

        if (rb != null && (rb.gameObject.CompareTag("Holder") || rb.gameObject.CompareTag("Box")))
        {
            rb.gravityScale = 1;
        }
    }

    public void SetDirection()
    {
        Debug.Log(" ���� ���");
        isRight = !isRight;
        moveDirection = isRight ? Vector2.right : Vector2.left;
        ApplyFlip();
    }

    // isRight ���� ���� X�� �������� ����
    private void ApplyFlip()
    {
        float flip = isRight ? 1f : -1f;
        transform.localScale = new Vector3(_originalScale.x * flip,
                                           _originalScale.y,
                                           _originalScale.z);
    }
}
