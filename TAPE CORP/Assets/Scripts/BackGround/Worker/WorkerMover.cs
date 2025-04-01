using UnityEngine;

public class WorkerMover : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float outOfCameraTimer = 0f; // ī�޶� �ۿ� �ӹ� �ð�

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void MoveTo(Vector3 target)
    {
        moveSpeed = Random.Range(3f, 5f);
        targetPosition = target;
        isMoving = true;

        if (animator != null)
            animator.SetBool("IsMoving", true);

        Vector3 dir = target - transform.position;
        GetComponent<SpriteRenderer>().flipX = dir.x < 0f;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // ī�޶� �� üũ
        if (IsInCameraView())
        {
            outOfCameraTimer = 0f; // �ȿ� �������� ����
        }
        else
        {
            outOfCameraTimer += Time.deltaTime;
            if (outOfCameraTimer >= 1f)
            {
                Destroy(gameObject); // 1�� �̻� �ۿ� �־����� ����
                return;
            }
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            isMoving = false;

            if (animator != null)
                animator.SetBool("IsMoving", false);

            Destroy(gameObject, 0.5f);
        }
    }

    bool IsInCameraView()
    {
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        return vp.x >= 0 && vp.x <= 1 && vp.y >= 0 && vp.y <= 1 && vp.z >= 0;
    }
}
