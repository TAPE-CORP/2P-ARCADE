// BasePlayerController.cs
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
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

    [Header("Sprites")]
    [Tooltip("위로 이동할 때 사용할 스프라이트")]
    public Sprite upSprite;
    [Tooltip("아래로 이동할 때 사용할 스프라이트")]
    public Sprite downSprite;
    [Tooltip("좌우 이동할 때 사용할 스프라이트")]
    public Sprite sideSprite;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer가 필요합니다.");

        // 초기 스프라이트 설정
        spriteRenderer.sprite = downSprite;
    }

    void Update()
    {
        // 입력 처리
        moveInput = Vector2.zero;
        if (Input.GetKey(leftKey)) moveInput.x = -1f;
        if (Input.GetKey(rightKey)) moveInput.x = 1f;
        if (Input.GetKey(upKey)) moveInput.y = 1f;
        if (Input.GetKey(downKey)) moveInput.y = -1f;
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        // 스프라이트 교체
        if (moveInput.y > 0.1f)
        {
            spriteRenderer.sprite = upSprite;
            spriteRenderer.flipX = false;
        }
        else if (moveInput.y < -0.1f)
        {
            spriteRenderer.sprite = downSprite;
            spriteRenderer.flipX = false;
        }
        else if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            spriteRenderer.sprite = sideSprite;
            // 좌우 반전 로직 수정: 오른쪽 이동 시만 반전
            spriteRenderer.flipX = moveInput.x > 0f;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
        }
    }
}