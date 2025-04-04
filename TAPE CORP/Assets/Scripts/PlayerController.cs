using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("방향 고정 키 설정")]
    public KeyCode directionLockKey = KeyCode.DownArrow; 
    public float moveSpeed = 5f;            // 최대 속도
    public float acceleration = 10f;        // 가속도
    public float deceleration = 15f;        // 감속도
    public float jumpForce = 7f;

    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode jumpKey = KeyCode.UpArrow;
    public KeyCode interactKey = KeyCode.DownArrow;

    public Transform targetToInteract;
    public RulerController interactionRuler;

    public Rigidbody2D rb;
    public bool isGrounded;

    private float currentVelocityX = 0f;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Update()
    {
        float targetVelocityX = 0f;

        // 방향 키 입력
        if (Input.GetKey(leftKey))
            targetVelocityX = -moveSpeed;
        else if (Input.GetKey(rightKey))
            targetVelocityX = moveSpeed;
        // 방향 반전 (잡기 중이면 방향 고정)
        bool isLocked = interactionRuler != null && interactionRuler.isGrabbing == true;

        if (!isLocked)
        {
            if (targetVelocityX > 0.1f)
                transform.localScale = new Vector3(1, 1, 1);
            else if (targetVelocityX < -0.1f)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // 관성 보간 처리
        bool isMovingInput = Mathf.Abs(targetVelocityX) > 0.01f;
        float appliedAccel = isMovingInput ? acceleration : deceleration;

        currentVelocityX = Mathf.MoveTowards(currentVelocityX, targetVelocityX, appliedAccel * Time.deltaTime);

        // 손 뗐을 때도 살짝 미끄러지게 유지
        if (!isMovingInput && Mathf.Abs(currentVelocityX) < 0.05f)
        {
            currentVelocityX = 0f; // 부드럽게 멈추게 보정
        }

        rb.velocity = new Vector2(currentVelocityX, rb.velocity.y);

        // 점프
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }


    public void OnCollisionStay2D(Collision2D collision)
    {
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

    public void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
