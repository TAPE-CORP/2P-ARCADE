using UnityEngine;

public class DoorFrameDropper : MonoBehaviour
{
    [Header("���� ����")]
    public float xStart = -5f;
    public float xEnd = 5f;
    public float ySpawn = 10f;

    [Header("��Ʋ ������Ʈ (�ڱ� �ڽ����� ���)")]
    public Rigidbody2D rb;

    private bool isLanded = false;

    void Start()
    {
        // ���� ��ġ�� �����ϰ� ����
        float xPos = Random.Range(xStart, xEnd);
        transform.position = new Vector3(xPos, ySpawn, 0f);

        // Rigidbody�� ���ٸ� ����
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLanded) return;

        // �ٴڰ��� �浹 �Ǵ� (�Ʒ� ���⿡���� �浹�� ����)
        if (collision.contacts[0].normal.y > 0.5f)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic; // �浹�� �����ϵ�, ����
            isLanded = true;
        }
    }
}
