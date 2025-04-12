using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DroneController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float followDistance = 2f;       // 멈춤 거리
    public float slowDownDistance = 4f;     // 감속 시작 거리
    public float moveSpeed = 3f;

    [Header("Floating")]
    public float floatSpeed = 2f;
    public float floatAmount = 0.2f;

    [Header("Tilting")]
    public float maxTiltAngle = 15f;
    public float tiltSmooth = 5f;

    private float floatTimer = 0f;
    private Rigidbody2D rb;
    public bool isStopped = false; // 타겟 근처에 도달했는지 여부

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void Update()
    {
        FollowTarget();


        ApplyTiltEffect();
        if (isStopped)
            transform.rotation = Quaternion.identity; // 정지 상태에서 회전 초기화
            //ApplyFloatingEffect();
    }

    void FollowTarget()
    {
        if (target == null)
        {
            return;
        }

        Vector2 direction = (target.position - transform.position);
        float distance = direction.magnitude;

        if (distance - followDistance < 0.05f)
        {
            if (!isStopped)
            {
                Debug.Log("Drone stopped. Close enough to target.");
                isStopped = true;
                floatTimer = 0f; // 떠오름 리셋
            }
            return;
        }

        // 이동 시작 시 상태 초기화
        isStopped = false;

        Vector2 moveDir = direction.normalized;

        // 거리 기반 감속 비율 적용
        float speedFactor = 1f;
        if (distance < slowDownDistance)
        {
            speedFactor = (distance - followDistance) / (slowDownDistance - followDistance);
            speedFactor = Mathf.Clamp01(speedFactor);
        }

        float finalSpeed = moveSpeed * speedFactor;
        rb.MovePosition(rb.position + moveDir * finalSpeed * Time.deltaTime);
    }

    void ApplyFloatingEffect()
    {
        floatTimer += Time.deltaTime * floatSpeed;
        float floatOffset = Mathf.Sin(floatTimer) * floatAmount;
        transform.position = new Vector3(transform.position.x, transform.position.y + floatOffset, transform.position.z);
    }

    void ApplyTiltEffect()
    {
        if (target == null || isStopped) return;

        float xDiff = target.position.x - transform.position.x;
        float tilt = Mathf.Clamp(-xDiff * maxTiltAngle, -maxTiltAngle, maxTiltAngle);
        Quaternion targetRot = Quaternion.Euler(0, 0, tilt);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * tiltSmooth);
    }
}
