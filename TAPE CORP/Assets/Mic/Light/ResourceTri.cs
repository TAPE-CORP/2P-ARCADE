using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ResourceTri : MonoBehaviour
{
    [Header("증가량 범위")]
    [Tooltip("최소 증가 값")]
    public float minIncrease = 2f;
    [Tooltip("최대 증가 값")]
    public float maxIncrease = 5f;

    void Awake()
    {
        // Collider2D를 트리거로 설정
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ResourceCon.instance != null)
        {
            float amount = Random.Range(minIncrease, maxIncrease);
            ResourceCon.instance.Gage += amount;
            Debug.Log($"ResourceTri: Gage +{amount:F1} → {ResourceCon.instance.Gage:F1}");
            Destroy(gameObject); // 자원 획득 후 오브젝트 삭제
        }
    }
}
