// BasePlayerController.cs
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BasePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("이동 속도")]
    public float moveSpeed = 5f;

    [Header("Key Bindings (WASD)")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // 플레이어 콜라이더가 트리거가 아니어야 하는 경우:
        // GetComponent<Collider2D>().isTrigger = false;
        // 적 쪽 콜라이더의 Is Trigger를 에디터에서 켜두세요.
    }

    void Update()
    {
        moveInput = Vector2.zero;
        if (Input.GetKey(leftKey)) moveInput.x = -1f;
        if (Input.GetKey(rightKey)) moveInput.x = 1f;
        if (Input.GetKey(upKey)) moveInput.y = 1f;
        if (Input.GetKey(downKey)) moveInput.y = -1f;
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    // 트리거 충돌 감지로 변경
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
        }
    }
}
