using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 1.5f; // 컨베이어 벨트의 속도
    public bool isRight = true;

    private Vector2 moveDirection;
    private Vector3 _originalScale;

    void Awake()
    {
        // 원본 스케일 저장
        _originalScale = transform.localScale;
        ApplyFlip();
    }

    void Update()
    {
        // 방향 설정 (스크롤 방향과 일치)
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
        Debug.Log(" 방향 토글");
        isRight = !isRight;
        moveDirection = isRight ? Vector2.right : Vector2.left;
        ApplyFlip();
    }

    // isRight 값에 따라 X축 스케일을 반전
    private void ApplyFlip()
    {
        float flip = isRight ? 1f : -1f;
        transform.localScale = new Vector3(_originalScale.x * flip,
                                           _originalScale.y,
                                           _originalScale.z);
    }
}
