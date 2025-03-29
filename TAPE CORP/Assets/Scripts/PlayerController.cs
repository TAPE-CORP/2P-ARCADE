using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public bool isPlayerOne = true;

    public Transform P1;
    public Transform P2;
    private Rigidbody2D rb;
    private bool isGrounded;

    public RulerController p1_TO_p2;
    public RulerController p2_TO_p1;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float move = 0f;

        if (isPlayerOne)
        {
            // 1P - 방향키
            if (Input.GetKey(KeyCode.LeftArrow))
                move = -1f;
            else if (Input.GetKey(KeyCode.RightArrow))
                move = 1f;

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && !p1_TO_p2.enabled && Vector2.Distance(P1.position, P2.position) < 1f)
            {
                p1_TO_p2.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && p1_TO_p2.enabled)
            {
                p1_TO_p2.enabled = false;
            }
        }
        else
        {
            // 2P - WASD
            if (Input.GetKey(KeyCode.A))
                move = -1f;
            else if (Input.GetKey(KeyCode.D))
                move = 1f;

            if (Input.GetKeyDown(KeyCode.W) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            if (Input.GetKeyDown(KeyCode.S) && !p2_TO_p1.enabled && Vector2.Distance(P1.position, P2.position) < 1f)
            {
                p2_TO_p1.enabled = true;
            }
            else if (Input.GetKeyDown(KeyCode.S) && p2_TO_p1.enabled)
            {
                p2_TO_p1.enabled = false;
            }
        }
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥에 닿았는지 체크
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 접촉 유지 중에도 바닥이면 isGrounded 유지
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }

        isGrounded = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
