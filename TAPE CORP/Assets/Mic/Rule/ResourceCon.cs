using UnityEngine;

public class ResourceCon : MonoBehaviour
{
    public static ResourceCon instance;

    [Header("Resource Settings")]
    public float Gage = 100f;

    [Header("Minus Speed 설정")]
    [Tooltip("움직임 없을 때의 기본 감소 속도")]
    public float defaultMinusSpd = 1f;
    [Tooltip("움직이는 플레이어 1인당 곱해질 배수")]
    public float upSpd = 1.8f;

    public float minusSpd;

    void Awake()
    {
        instance = this;
        minusSpd = defaultMinusSpd;
    }

    void Update()
    {
        // 매 프레임 게이지 감소 전, 움직이는 Player 개수에 따라 minusSpd 재계산
        RecalculateMinusSpd();

        Gage -= minusSpd * Time.deltaTime;
        Debug.Log($"남은 자원 = {Gage:f1}, minusSpd = {minusSpd:f2}");
    }

    private void RecalculateMinusSpd()
    {
        // 태그가 "Player"인 모든 오브젝트를 찾는다
        var players = GameObject.FindGameObjectsWithTag("Player");
        int movingCount = 0;

        foreach (var p in players)
        {
            var rb = p.GetComponent<Rigidbody2D>();
            if (rb != null && rb.velocity.magnitude > 0.1f)
                movingCount++;
        }

        // defaultMinusSpd × upSpd^movingCount
        minusSpd = defaultMinusSpd * Mathf.Pow(upSpd, movingCount);
    }
}
