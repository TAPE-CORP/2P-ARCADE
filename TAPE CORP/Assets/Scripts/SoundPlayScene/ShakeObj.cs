using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ShakeObj : MonoBehaviour
{
    // 외부에서 할당할 weight (예: kg)
    [SerializeField] private float weight;

    [Header("무게 정규화 기준값")]
    [Tooltip("weight / maxWeight 로 intensity 를 계산합니다.")]
    [SerializeField] private float maxWeight = 10f;

    /// <summary>
    /// 외부에서 객체를 생성하면서 랜덤 또는 원하는 값으로 무게를 할당할 때 호출하세요.
    /// </summary>
    public void SetWeight(float w)
    {
        weight = w;
    }

    /// <summary>
    /// 플레이어가 이 오브젝트를 들어올릴 때, 혹은 충돌 시 호출됩니다.
    /// </summary>
    public void OnLifted()
    {
        if (SoundIO.Instance == null)
        {
            Debug.LogWarning("SoundIO 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // 설정된 weight / maxWeight 비율로 intensity 계산 (0~1)
        float intensity = Mathf.Clamp01(weight / maxWeight);

        // 사운드 재생
        SoundIO.Instance.PlayWithIntensity(intensity);
    }

    /// <summary>
    /// Map 레이어인 오브젝트와 충돌했을 때만 OnLifted를 호출합니다.
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        // "Map" 레이어 이름을 이용해 레이어 인덱스를 구함
        int mapLayer = LayerMask.NameToLayer("Map");
        if (collision.gameObject.layer == mapLayer)
        {
            OnLifted();
        }
    }
}
