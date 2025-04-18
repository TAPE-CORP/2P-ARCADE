using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BasePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Key Bindings")]
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode jumpKey = KeyCode.UpArrow;

    [Header("Ground Layer Mask")]
    public LayerMask groundLayerMask;

    [Tooltip("0=P1, 1=P2 로 매칭하세요")]
    public int playerIndex = 0;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private int groundContactCount = 0;
    private Vector3 initialScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
    }

    void Update()
    {
        // 좌우 입력
        horizontalInput = 0f;
        if (Input.GetKey(leftKey)) horizontalInput = -1f;
        if (Input.GetKey(rightKey)) horizontalInput = 1f;

        // 스케일 반전
        if (horizontalInput > 0f)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (horizontalInput < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);

        // 점프
        if (Input.GetKeyDown(jumpKey) && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void FixedUpdate()
    {
        // 실제 이동
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // 움직임 판단 (속도 크기로)
        bool isMoving = rb.velocity.magnitude > 0.1f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayerMask.value) != 0)
        {
            groundContactCount++;
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayerMask.value) != 0)
        {
            groundContactCount = Mathf.Max(groundContactCount - 1, 0);
            isGrounded = groundContactCount > 0;
        }
    }
}
