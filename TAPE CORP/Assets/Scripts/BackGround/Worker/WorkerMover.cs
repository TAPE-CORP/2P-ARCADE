using UnityEngine;

public class WorkerMover : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool hasLanded = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float outOfCameraTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 같은 Worker 레이어끼리 충돌 무시
        int workerLayer = LayerMask.NameToLayer("Worker");
        Physics2D.IgnoreLayerCollision(workerLayer, workerLayer);

        // Rigidbody 안정화
        if (TryGetComponent(out Rigidbody2D rb))
        {
            rb.freezeRotation = true;
        }
    }

    public void MoveHorizontallyAcrossScreen2D()
    {
        moveSpeed = Random.Range(2f, 4f);

        bool moveRight = Random.value > 0.5f;
        float moveDistance = 30f;
        float direction = moveRight ? 1f : -1f;

        targetPosition = transform.position + new Vector3(direction * moveDistance, 0f, 0f);
        isMoving = true;

        if (animator != null)
            animator.SetBool("IsMoving", true);

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction < 0f;
    }

    void OnEnable()
    {
        // 상태 초기화
        isMoving = false;
        hasLanded = false;
        outOfCameraTimer = 0f;

        if (animator != null)
            animator.SetBool("IsMoving", false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && collision.collider.CompareTag("Bridge"))
        {
            hasLanded = true;
            MoveHorizontallyAcrossScreen2D();
        }
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (IsInCameraView())
        {
            outOfCameraTimer = 0f;
        }
        else
        {
            outOfCameraTimer += Time.deltaTime;
            if (outOfCameraTimer >= 1f)
            {
                ReturnToPool();
                return;
            }
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            isMoving = false;

            if (animator != null)
                animator.SetBool("IsMoving", false);

            Invoke(nameof(ReturnToPool), 0.5f);
        }
    }

    bool IsInCameraView()
    {
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        return vp.x >= 0 && vp.x <= 1 && vp.y >= 0 && vp.y <= 1 && vp.z >= 0;
    }

    void ReturnToPool()
    {
        WorkerPoolManager.Instance.ReturnWorker(gameObject);
    }
}
