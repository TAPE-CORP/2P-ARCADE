using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class WaveRipple : MonoBehaviour
{
    [Header("파동 설정")]
    [Tooltip("파동이 완전히 퍼지는 시간 (초)")]
    public float expandDuration = 1f;
    [Tooltip("파동 최대 반경 (월드 단위)")]
    public float maxRadius = 5f;

    [Header("판정 대상 태그")]
    [Tooltip("파동에 반응할 오브젝트의 태그")]
    public string targetTag = "Sound";

    [Header("잔상 데칼")]
    [Tooltip("빛 잔상 데칼 프리팹 (DecalFade 스크립트 포함)")]
    public GameObject lightDecalPrefab;
    [Tooltip("잔상 데칼을 생성할 최대 개수")]
    public int maxDecals = 30;
    [Tooltip("잔상 위치 산포 거리(월드 단위)")]
    public float decalScatter = 0.1f;

    private CircleCollider2D _trigger;
    private Rigidbody2D _rb;
    private Vector2 _center;
    private float _worldRadius;

    void Awake()
    {
        _trigger = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();

        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.useFullKinematicContacts = true;

        _trigger.isTrigger = true;
        _trigger.radius = 0f;
    }

    /// <summary>
    /// MicDecibelMeter 등에서 호출하세요.
    /// maxRadius: 파동이 퍼질 최대 반경
    /// </summary>
    public void Initialize(float maxRadius)
    {
        this.maxRadius = maxRadius;
        _center = transform.position;
        transform.localScale = Vector3.zero;
        StartCoroutine(DoRipple());
    }

    private IEnumerator DoRipple()
    {
        float t = 0f;
        while (t < expandDuration)
        {
            t += Time.deltaTime;
            float n = t / expandDuration;

            // 현재 반경
            _worldRadius = Mathf.Lerp(0f, maxRadius, n);
            // 비주얼과 트리거 반경 동기화
            transform.localScale = Vector3.one * _worldRadius;
            _trigger.radius = _worldRadius;

            yield return null;
        }

        PaintLightTrails();
        Destroy(gameObject);
    }

    // Sound 태그가 붙은 물체에만 데칼을 찍습니다.
    private void PaintLightTrails()
    {
        var hits = Physics2D.OverlapCircleAll(_center, _worldRadius);
        if (hits.Length == 0 || lightDecalPrefab == null) return;

        foreach (var col in hits)
        {
            if (!col.CompareTag(targetTag))
                continue;

            // 파동의 중심(_center)과 해당 콜라이더 간의 가장 가까운 지점을 가져옵니다.
            Vector2 contactPoint = col.ClosestPoint(_center);

            // z-축은 콜라이더의 z보다 살짝 앞에 배치
            Vector3 decalPos = new Vector3(
                contactPoint.x,
                contactPoint.y,
                col.transform.position.z - 0.01f
            );

            Instantiate(lightDecalPrefab, decalPos, Quaternion.identity, col.transform);
        }
    }
}
