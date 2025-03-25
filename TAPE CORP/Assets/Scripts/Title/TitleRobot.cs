using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TitleRobotController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float bounceIntensity = 2f;
    public float jumpForce = 5f;

    public float bounceBeatInterval = 1f;
    public float jumpBeatIntervalMin = 1f;
    public float jumpBeatIntervalMax = 2f;

    private Rigidbody2D rb;
    private float nextBounceTime;
    private float nextJumpTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Rigidbody 설정 보완
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true; // 회전 방지

        nextBounceTime = Time.time + bounceBeatInterval;
        nextJumpTime = Time.time + Random.Range(jumpBeatIntervalMin, jumpBeatIntervalMax);
    }

    void Update()
    {
        // 좌우 이동 처리
        float move = Mathf.Sin(Time.time * moveSpeed);
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        // 들썩임 처리
        if (Time.time >= nextBounceTime)
        {
            rb.AddForce(Vector2.up * bounceIntensity, ForceMode2D.Impulse);
            nextBounceTime = Time.time + bounceBeatInterval;
        }

        // 랜덤 점프 처리
        if (Time.time >= nextJumpTime)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            nextJumpTime = Time.time + Random.Range(jumpBeatIntervalMin, jumpBeatIntervalMax);
        }
    }

    // 땅에서 충돌 확인
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Ground Hit");
        }
    }
}
