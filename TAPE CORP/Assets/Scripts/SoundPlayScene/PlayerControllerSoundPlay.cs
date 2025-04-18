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
    [Tooltip("Ű�� ���� ���� ����� Box �±� ������Ʈ�� �����ϴ�.")]
    public KeyCode pickUpKey = KeyCode.RightShift;
    [Tooltip("Box �±� ������Ʈ �˻� �ݰ�")]
    public float pickUpRadius = 2f;
    [Tooltip("�÷��̾�� ������Ʈ ���� �߰� ���� �Ÿ�")]
    public float extraHoldSpace = 0.5f;

    [Header("P2 Holder Detection")]
    [Tooltip("Holder ���̾ ���� �ݶ��̴��� Stay �ν�")]
    public LayerMask holderLayerMask;

    [Header("�̵� �� minusSpd ���")]
    [Tooltip("�⺻ minusSpd�� �������� ��� ��")]
    public float upSpd = 1.8f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private int facingDirection = 1;

    // minusSpd �⺻�� �����
    private float defaultMinusSpd;

    // P1 ����
    private GameObject heldObject;
    private ShakeObj heldShakeObj;
    private Collider2D[] heldColliders;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ResourceCon �� minusSpd �⺻���� �� ���� ����
        defaultMinusSpd = ResourceCon.instance.minusSpd;
    }

    void Update()
    {
        if (playerType == PlayerType.P1)
        {
            // ���̽� ���� ����
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) facingDirection = -1;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) facingDirection = 1;

            // �̵� ���̸� �⺻�� * upSpd, ���߸� �⺻������ ����
            float moveInput = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                ResourceCon.instance.minusSpd = defaultMinusSpd * upSpd;
                Debug.Log("�̵� ��: minusSpd = " + ResourceCon.instance.minusSpd);
            }
            else
            {
                ResourceCon.instance.minusSpd = defaultMinusSpd;
            }

            // ���� ���
            if (Input.GetKeyDown(pickUpKey) && heldObject == null)
                TryPickUp();

            // ����
            if (Input.GetKeyUp(pickUpKey) && heldObject != null)
                ReleaseHeldObject();
        }
    }
    private void TryPickUp()
    {
        // �ݰ� �� Box �±� ����
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

        // �÷��̾�� �ڽ� �� �浹 ����
        foreach (var boxCol in heldColliders)
            Physics2D.IgnoreCollision(playerCollider, boxCol, true);

        // Sprite ũ�� ���
        var playerSize = spriteRenderer.bounds.size;
        var boxSr = heldObject.GetComponent<SpriteRenderer>();
        var boxSize = boxSr.bounds.size;

        // �θ�-�ڽ� ���� ���� �� ��ġ ���� (���� ���� extraHoldSpace ����)
        heldObject.transform.SetParent(transform);
        float offsetX = facingDirection * (playerSize.x / 2f + boxSize.x / 2f + extraHoldSpace);
        heldObject.transform.localPosition = new Vector3(offsetX, 0f, 0f);

        // ���� ���
        if (heldObject.TryGetComponent<Rigidbody2D>(out var heldRb))
            heldRb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void ReleaseHeldObject()
    {
        // ���� ��� ���� �� �浹 ����
        if (heldObject.TryGetComponent<Rigidbody2D>(out var heldRb))
            heldRb.bodyType = RigidbodyType2D.Dynamic;

        foreach (var boxCol in heldColliders)
            Physics2D.IgnoreCollision(playerCollider, boxCol, false);

        // �θ� ����
        heldObject.transform.SetParent(null);

        // �ʱ�ȭ
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
                Debug.Log("2P: Holder ���̾� �ݶ��̴��� Stay ��");
                // �߰� ���� ���� ����
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
