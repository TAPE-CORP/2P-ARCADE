using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class WaveRipple : MonoBehaviour
{
    [Header("�ĵ� ����")]
    [Tooltip("�ĵ��� ������ ������ �ð� (��)")]
    public float expandDuration = 1f;
    [Tooltip("�ĵ� �ִ� �ݰ� (���� ����)")]
    public float maxRadius = 5f;
    [Tooltip("�� ���̾� ����ũ")]
    public LayerMask wallLayerMask;

    [Header("�ܻ� ��Į")]
    [Tooltip("�� �ܻ� ��Į ������ (DecalFade ��ũ��Ʈ ����)")]
    public GameObject lightDecalPrefab;
    [Tooltip("�ܻ� ��Į�� ������ �ִ� ����")]
    public int maxDecals = 30;
    [Tooltip("�ܻ� ��ġ ���� �Ÿ�(���� ����)")]
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
    /// MicDecibelMeter ��� ȣ���ϼ���.
    /// maxRadius: �ĵ��� ���� �ִ� �ݰ�
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

            // ���� �ݰ�
            _worldRadius = Mathf.Lerp(0f, maxRadius, n);
            // ���־�� Ʈ���� �ݰ� ����ȭ
            transform.localScale = Vector3.one * _worldRadius;
            _trigger.radius = _worldRadius;

            yield return null;
        }

        PaintLightTrails();
        Destroy(gameObject);
    }

    // WaveRipple.PaintLightTrails ����
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
