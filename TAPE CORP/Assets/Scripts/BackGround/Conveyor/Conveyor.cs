using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 1.5f; // �����̾� ��Ʈ�� �ӵ�
    public bool isRight = true;
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Rigidbody2D ������Ʈ�� ���� ��ü�� ���ؼ��� �ӵ��� ����
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // �����̾� ��Ʈ�� �������� �ӵ� ����
            Vector2 velocity = (isRight ? Vector2.right * Time.deltaTime : Vector2.left * Time.deltaTime);
            collision.transform.Translate(velocity * speed);
        }
    }
}
