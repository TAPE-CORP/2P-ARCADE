using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("방향 고정 키 설정")]
    public KeyCode directionLockKey = KeyCode.DownArrow;
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float jumpForce = 7f;

    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode jumpKey = KeyCode.UpArrow;
    public KeyCode interactKey = KeyCode.DownArrow;

    [Header("컨베이어 토글 키")]
    public KeyCode toggleConveyorKeyA = KeyCode.A;
    public KeyCode toggleConveyorKeyB = KeyCode.B;

    [Header("제어할 Conveyor들 (태그로 자동 수집)")]
    public List<Conveyor> conveyors = new List<Conveyor>();

    public Transform targetToInteract;
    public RulerController interactionRuler;

    public Rigidbody2D rb;
    public bool isGrounded;

    private float currentVelocityX = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // "Conveyor" 태그가 설정된 모든 오브젝트에서 Conveyor 컴포넌트를 찾아 리스트에 추가
        var convObjects = GameObject.FindGameObjectsWithTag("Convayor");
        conveyors = new List<Conveyor>();
        foreach (var go in convObjects)
        {
            var conv = go.GetComponent<Conveyor>();
            if (conv != null)
                conveyors.Add(conv);
        }
    }

    void Update()
    {
        // ===== 기존 이동/점프 로직 =====
        float targetVelocityX = 0f;
        if (Input.GetKey(leftKey))
            targetVelocityX = -moveSpeed;
        else if (Input.GetKey(rightKey))
            targetVelocityX = moveSpeed;

        bool isLocked = interactionRuler != null && interactionRuler.isGrabbing;
        if (!isLocked)
        {
            if (targetVelocityX > 0.1f)
                transform.localScale = Vector3.one;
            else if (targetVelocityX < -0.1f)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        bool isMovingInput = Mathf.Abs(targetVelocityX) > 0.01f;
        float appliedAccel = isMovingInput ? acceleration : deceleration;
        currentVelocityX = Mathf.MoveTowards(currentVelocityX, targetVelocityX, appliedAccel * Time.deltaTime);

        if (!isMovingInput && Mathf.Abs(currentVelocityX) < 0.05f)
            currentVelocityX = 0f;

        rb.velocity = new Vector2(currentVelocityX, rb.velocity.y);

        if (Input.GetKeyDown(jumpKey) && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // ===== Conveyor 방향 토글 =====
        if (Input.GetKeyDown(toggleConveyorKeyA) || Input.GetKeyDown(toggleConveyorKeyB))
        {
            foreach (var conv in conveyors)
                conv.isRight = !conv.isRight;

            Debug.Log("모든 Conveyor 방향 토글 완료");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        isGrounded = false;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
