using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 1.5f; // 컨베이어 벨트의 속도
    public bool isRight = true;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

        if (rb != null && rb.gameObject.CompareTag("Holder"))
        {
            // 중력 제거
            rb.gravityScale = 0;

            // 로컬 기준 오른쪽 방향으로 velocity 설정
            Vector2 localRight = transform.right.normalized;
            rb.velocity = localRight * speed;
        }
        else if (rb != null)
        {
            // 이 부분은 절대 건드리지 않음
            Vector2 velocity = (isRight ? Vector2.right * Time.deltaTime : Vector2.left * Time.deltaTime);
            collision.transform.Translate(velocity * speed);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

        if (rb != null && rb.gameObject.CompareTag("Holder"))
        {
            // 중력 다시 적용
            rb.gravityScale = 1;
        }
    }
}
