using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DoorFrame : MonoBehaviour
{
    public LayerMask groundMask;
    private Rigidbody2D rb;
    private bool settled = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (settled) return;

        // 바닥과 충돌했는지 체크
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundMask);
        if (hit.collider != null)
        {
            Settle();
        }
    }

    void Settle()
    {
        settled = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        //  각도 강제 정렬
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        //  물리 영향 제거
        rb.bodyType = RigidbodyType2D.Static;

        Debug.Log("DoorFrame settled and locked.");
    }
}
