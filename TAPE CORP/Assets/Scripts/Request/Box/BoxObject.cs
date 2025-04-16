using UnityEngine;

public class BoxObject : MonoBehaviour
{
    public float areaSize;
    public GameObject packedPrefab;
    public int score;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb == null)
            Debug.LogError("Rigidbody2D�� �����ϴ�.");
        if (sr == null)
            Debug.LogError("SpriteRenderer�� �����ϴ�.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!CompareTag("Packed") && collision.gameObject.CompareTag("Obj") && rb.isKinematic)
        {
            Debug.Log("���� �õ�");

            SpawnedObject so = collision.gameObject.GetComponent<SpawnedObject>();
            if (so == null || so.data == null)
            {
                Debug.LogWarning("SpawnedObject �Ǵ� Data�� �����ϴ�.");
                return;
            }

            Vector2 size = so.data.size;
            float objectArea = size.x * size.y;

            if (objectArea > areaSize)
            {
                Debug.Log("�ʹ� ŭ: ���� �Ұ�");
                return;
            }

            float difference = Mathf.Abs(areaSize - objectArea);
            score = Mathf.RoundToInt((1f / (difference + 0.01f)) * 100f);
            Debug.Log($"���� ����: ���� {score}");

            if (packedPrefab != null)
            {
                SpriteRenderer packedSR = packedPrefab.GetComponent<SpriteRenderer>();
                if (packedSR != null && sr != null)
                {
                    sr.sprite = packedSR.sprite;
                    tag = "Packed"; // �±� ����
                    collision.gameObject.SetActive(false); // �浹�� Obj ��Ȱ��ȭ
                }
                else
                {
                    Debug.LogWarning("packedPrefab�� SpriteRenderer�� ���ų� ���� ������Ʈ�� ����.");
                }
            }
        }
    }

    void OnDestroy()
    {
        if (CompareTag("Packed"))
        {
            ScoreSystem.Instance?.AddScore(score);
            Debug.Log("Packed ���ŵ� �� ���� ȹ��: " + score);
        }
    }
}
