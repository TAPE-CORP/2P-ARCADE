using UnityEngine;
using UnityEngine.Rendering.Universal;  // Light2D 사용을 위해

[RequireComponent(typeof(Collider2D))]
public class ResourceTri : MonoBehaviour
{
    [Header("증가량 범위")]
    [Tooltip("최소 증가 값")]
    public float minIncrease = 2f;
    [Tooltip("최대 증가 값")]
    public float maxIncrease = 5f;

    [Header("번개 플래시 설정")]
    [Tooltip("플래시될 최대 밝기")]
    public float flashIntensity = 5f;
    [Tooltip("플래시 지속 시간 (초)")]
    public float flashDuration = 0.1f;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ResourceCon.instance != null)
        {
            // 자원 증가
            float amount = Random.Range(minIncrease, maxIncrease);
            ResourceCon.instance.Gage += amount;
            Debug.Log($"ResourceTri: Gage +{amount:F1} → {ResourceCon.instance.Gage:F1}");

            // ResourceCon 쪽으로 플래시 요청
            ResourceCon.instance.RequestFlash(flashIntensity, flashDuration);

            // 획득 후 삭제
            Destroy(gameObject);
        }
    }
}
