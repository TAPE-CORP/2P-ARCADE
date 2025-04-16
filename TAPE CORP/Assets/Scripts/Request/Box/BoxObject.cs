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
            Debug.LogError("Rigidbody2D가 없습니다.");
        if (sr == null)
            Debug.LogError("SpriteRenderer가 없습니다.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!CompareTag("Packed") && collision.gameObject.CompareTag("Obj") && rb.isKinematic)
        {
            Debug.Log("포장 시도");

            SpawnedObject so = collision.gameObject.GetComponent<SpawnedObject>();
            if (so == null || so.data == null)
            {
                Debug.LogWarning("SpawnedObject 또는 Data가 없습니다.");
                return;
            }

            Vector2 size = so.data.size;
            float objectArea = size.x * size.y;

            if (objectArea > areaSize)
            {
                Debug.Log("너무 큼: 포장 불가");
                return;
            }

            float difference = Mathf.Abs(areaSize - objectArea);
            score = Mathf.RoundToInt((1f / (difference + 0.01f)) * 100f);
            Debug.Log($"포장 성공: 점수 {score}");

            if (packedPrefab != null)
            {
                SpriteRenderer packedSR = packedPrefab.GetComponent<SpriteRenderer>();
                if (packedSR != null && sr != null)
                {
                    sr.sprite = packedSR.sprite;
                    tag = "Packed"; // 태그 변경
                    collision.gameObject.SetActive(false); // 충돌한 Obj 비활성화
                }
                else
                {
                    Debug.LogWarning("packedPrefab에 SpriteRenderer가 없거나 현재 오브젝트에 없음.");
                }
            }
        }
    }

    void OnDestroy()
    {
        if (CompareTag("Packed"))
        {
            ScoreSystem.Instance?.AddScore(score);
            Debug.Log("Packed 제거됨 → 점수 획득: " + score);
        }
    }
}
