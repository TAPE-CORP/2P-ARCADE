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
    [Tooltip("벽 레이어 마스크")]
    public LayerMask wallLayerMask;

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

    // WaveRipple.PaintLightTrails 내부
    private void PaintLightTrails()
    {
        var hits = Physics2D.OverlapCircleAll(_center, _worldRadius, wallLayerMask);
        if (hits.Length == 0 || lightDecalPrefab == null) return;

        int placed = 0;
        while (placed < maxDecals)
        {
            Vector2 rnd = Random.insideUnitCircle * _worldRadius + _center;
            foreach (var wall in hits)
            {
                if (wall.OverlapPoint(rnd))
                {
                    Vector3 pos = new Vector3(
                        rnd.x,
                        rnd.y,
                        wall.transform.position.z - 0.01f
                    );
                    var go = Instantiate(lightDecalPrefab, pos, Quaternion.identity, wall.transform);
                    placed++;
                    break;
                }
            }
        }
    }
}
