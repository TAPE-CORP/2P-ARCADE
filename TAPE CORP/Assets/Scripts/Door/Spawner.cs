using UnityEngine;

public class DoorFrameDropper : MonoBehaviour
{
    [Header("스폰 범위")]
    public float xStart = -5f;
    public float xEnd = 5f;
    public float ySpawn = 10f;

    [Header("문틀 오브젝트 (자기 자신으로 사용)")]
    public Rigidbody2D rb;

    private bool isLanded = false;

    void Start()
    {
        // 시작 위치를 랜덤하게 지정
        float xPos = Random.Range(xStart, xEnd);
        transform.position = new Vector3(xPos, ySpawn, 0f);

        // Rigidbody가 없다면 붙임
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLanded) return;

        // 바닥과의 충돌 판단 (아래 방향에서의 충돌만 인정)
        if (collision.contacts[0].normal.y > 0.5f)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic; // 충돌은 유지하되, 고정
            isLanded = true;
        }
    }
}
