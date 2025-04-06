using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 1.5f; // 컨베이어 벨트의 속도
    public bool isRight = true;
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Rigidbody2D 컴포넌트를 가진 물체에 대해서만 속도를 적용
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 컨베이어 벨트의 방향으로 속도 적용
            Vector2 velocity = (isRight ? Vector2.right * Time.deltaTime : Vector2.left * Time.deltaTime);
            collision.transform.Translate(velocity * speed);
        }
    }
}
