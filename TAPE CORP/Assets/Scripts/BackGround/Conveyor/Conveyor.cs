using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 1.5f; // �����̾� ��Ʈ�� �ӵ�
    public bool isRight = true;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

        if (rb != null && rb.gameObject.CompareTag("Holder"))
        {
            // �߷� ����
            rb.gravityScale = 0;

            // ���� ���� ������ �������� velocity ����
            Vector2 localRight = transform.right.normalized;
            rb.velocity = localRight * speed;
        }
        else if (rb != null)
        {
            // �� �κ��� ���� �ǵ帮�� ����
            Vector2 velocity = (isRight ? Vector2.right * Time.deltaTime : Vector2.left * Time.deltaTime);
            collision.transform.Translate(velocity * speed);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

        if (rb != null && rb.gameObject.CompareTag("Holder"))
        {
            // �߷� �ٽ� ����
            rb.gravityScale = 1;
        }
    }
}
