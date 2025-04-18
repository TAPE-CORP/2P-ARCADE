using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class PlayerControllerSoundPlay : MonoBehaviour
{
    public enum PlayerType { P1, P2 }

    [Header("Player Settings")]
    public PlayerType playerType = PlayerType.P1;

    [Header("P1 Pickup Settings")]
    [Tooltip("키를 눌러 가장 가까운 Box 태그 오브젝트를 집습니다.")]
    public KeyCode pickUpKey = KeyCode.RightShift;
    [Tooltip("Box 태그 오브젝트 검색 반경")]
    public float pickUpRadius = 2f;
    [Tooltip("플레이어와 오브젝트 사이 추가 여유 거리")]
    public float extraHoldSpace = 0.5f;

    [Header("P2 Holder Detection")]
    [Tooltip("Holder 레이어에 속한 콜라이더와 Stay 인식")]
    public LayerMask holderLayerMask;

    [Header("이동 시 minusSpd 배수")]
    [Tooltip("기본 minusSpd에 곱해지는 배수 값")]
    public float upSpd = 1.8f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private int facingDirection = 1;

    // minusSpd 기본값 저장용
    private float defaultMinusSpd;

    // P1 전용
    private GameObject heldObject;
    private ShakeObj heldShakeObj;
    private Collider2D[] heldColliders;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ResourceCon 쪽 minusSpd 기본값을 한 번만 저장
        defaultMinusSpd = ResourceCon.instance.minusSpd;
    }

    void Update()
    {
        if (playerType == PlayerType.P1)
        {
            // 페이싱 방향 갱신
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) facingDirection = -1;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) facingDirection = 1;

            // 이동 중이면 기본값 * upSpd, 멈추면 기본값으로 리셋
            float moveInput = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                ResourceCon.instance.minusSpd = defaultMinusSpd * upSpd;
                Debug.Log("이동 중: minusSpd = " + ResourceCon.instance.minusSpd);
            }
            else
            {
                ResourceCon.instance.minusSpd = defaultMinusSpd;
            }

            // 집기 토글
            if (Input.GetKeyDown(pickUpKey) && heldObject == null)
                TryPickUp();

            // 놓기
            if (Input.GetKeyUp(pickUpKey) && heldObject != null)
                ReleaseHeldObject();
        }
    }
    private void TryPickUp()
    {
        // 반경 내 Box 태그 수집
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickUpRadius);
        Collider2D nearest = null;
        float minDist = float.MaxValue;

        foreach (var col in hits)
        {
            if (col.CompareTag("Box"))
            {
                float d = Vector2.Distance(transform.position, col.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = col;
                }
            }
        }

        if (nearest == null) return;

        heldObject = nearest.gameObject;
        heldShakeObj = heldObject.GetComponent<ShakeObj>();
        heldColliders = heldObject.GetComponents<Collider2D>();

        // 플레이어와 박스 간 충돌 무시
        foreach (var boxCol in heldColliders)
            Physics2D.IgnoreCollision(playerCollider, boxCol, true);

        // Sprite 크기 계산
        var playerSize = spriteRenderer.bounds.size;
        var boxSr = heldObject.GetComponent<SpriteRenderer>();
        var boxSize = boxSr.bounds.size;

        // 부모-자식 관계 설정 후 위치 조정 (여유 공간 extraHoldSpace 포함)
        heldObject.transform.SetParent(transform);
        float offsetX = facingDirection * (playerSize.x / 2f + boxSize.x / 2f + extraHoldSpace);
        heldObject.transform.localPosition = new Vector3(offsetX, 0f, 0f);

        // 물리 잠금
        if (heldObject.TryGetComponent<Rigidbody2D>(out var heldRb))
            heldRb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void ReleaseHeldObject()
    {
        // 물리 잠금 해제 및 충돌 복원
        if (heldObject.TryGetComponent<Rigidbody2D>(out var heldRb))
            heldRb.bodyType = RigidbodyType2D.Dynamic;

        foreach (var boxCol in heldColliders)
            Physics2D.IgnoreCollision(playerCollider, boxCol, false);

        // 부모 해제
        heldObject.transform.SetParent(null);

        // 초기화
        heldObject = null;
        heldShakeObj = null;
        heldColliders = null;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (playerType == PlayerType.P2)
        {
            if (((1 << collision.gameObject.layer) & holderLayerMask.value) != 0)
            {
                Debug.Log("2P: Holder 레이어 콜라이더와 Stay 중");
                // 추가 동작 구현 가능
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (playerType == PlayerType.P1)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickUpRadius);
        }
    }
}
