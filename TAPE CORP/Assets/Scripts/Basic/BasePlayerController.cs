using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
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
        // 중력 해제
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 입력 읽기
        moveInput = Vector2.zero;
        if (Input.GetKey(leftKey)) moveInput.x = -1f;
        if (Input.GetKey(rightKey)) moveInput.x = 1f;
        if (Input.GetKey(upKey)) moveInput.y = 1f;
        if (Input.GetKey(downKey)) moveInput.y = -1f;

        // 대각선일 때 움직임 정규화
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();
    }

    void FixedUpdate()
    {
        // Rigidbody2D로 이동
        rb.velocity = moveInput * moveSpeed;
    }
}
