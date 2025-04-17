using System.Collections;
using System.Collections.Generic;
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

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private int groundContactCount = 0;

    // 초기 스케일을 저장해 둡니다.
    private Vector3 initialScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
    }

    void Update()
    {
        // 좌우 입력 처리
        horizontalInput = 0f;
        if (Input.GetKey(leftKey))
            horizontalInput = -1f;
        else if (Input.GetKey(rightKey))
            horizontalInput = 1f;

        // 이동 방향에 따라 localScale.x 를 반전
        if (horizontalInput > 0f)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (horizontalInput < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);

        // 점프 처리
        if (Input.GetKeyDown(jumpKey) && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
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
            isGrounded = (groundContactCount > 0);
        }
    }
}
