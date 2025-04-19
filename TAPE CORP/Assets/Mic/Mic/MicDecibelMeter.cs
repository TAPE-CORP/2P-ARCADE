using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class MicDecibelMeter : MonoBehaviour
{
    [Header("���� �߻� �Ӱ谪")]
    [Tooltip("RMS ���� �� �Ӱ谪 �̻��� ���� ���̸� �߻��մϴ�.")]
    public float rmsThreshold = 0.02f;

    [Header("����ũ ����")]
    public string micDevice = "";
    public int sampleRate = 44100;
    public int sampleWindow = 1024;

    [Header("�ĵ� ȿ�� ����")]
    public GameObject wavePrefab;
    [Tooltip("RMS * �� �� = �ĵ� �ִ� �ݰ�")]
    public float waveScaleFactor = 0.05f;
    public float spawnInterval = 0.2f;

    [Header("���� ���� ����")]
    [Tooltip("RMS * �� �� = ���� ����")]
    public float rayLengthFactor = 5f;
    [Tooltip("���� �ּ� ���� (�浹 ���� Ȯ��)")]
    public float minRayLength = 1f;
    public int rayCount = 16;
    [Tooltip("�浹 �˻��� �� ���̾�")]
    public LayerMask targetLayerMask;

    [Header("����Ʈ Ǯ ����")]
    public GameObject lightPrefab;
    public int poolCapacity = 32;

    [Header("���� �߻� ���� Ʈ������")]
    public Transform spawnMarker;

    [Header("���� �ڿ� ���� ����")]
    [Tooltip("RMS * �� �� * spawnInterval ��ŭ Gage�� �����մϴ�.")]
    public float soundDrainFactor = 5f;

    private AudioSource _audioSource;
    private float[] _samples;
    private float _spawnTimer;
    private ObjectPool<GameObject> _lightPool;

    void Awake()
    {
        _samples = new float[sampleWindow];
        if (spawnMarker == null)
            spawnMarker = transform;

        _lightPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var go = Instantiate(lightPrefab);
                var lf = go.GetComponent<Light2DFade>();
                lf.onFadeComplete = ReleaseLight;
                go.SetActive(false);
                return go;
            },
            actionOnGet: go =>
            {
                go.transform.SetParent(null);
                go.SetActive(true);
            },
            actionOnRelease: go => go.SetActive(false),
            actionOnDestroy: go => Destroy(go),
            collectionCheck: false,
            defaultCapacity: poolCapacity,
            maxSize: poolCapacity * 2
        );
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("����ũ ��ġ�� ã�� �� �����ϴ�.");
            enabled = false;
            return;
        }
        if (string.IsNullOrEmpty(micDevice))
            micDevice = Microphone.devices[0];

        _audioSource.clip = Microphone.Start(micDevice, true, 1, sampleRate);
        _audioSource.loop = true;
        _audioSource.mute = true;
        while (Microphone.GetPosition(micDevice) <= 0) { }
        _audioSource.Play();

        Debug.Log($"[MicDecibelMeter] ����ũ ����: {micDevice}, ���� ������: {sampleWindow}");
    }

    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            _spawnTimer = spawnInterval;
            return;
        }

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer < spawnInterval) return;
        _spawnTimer = 0f;

        float rms = GetRMS();
        Debug.Log($"[MicDecibelMeter] RMS: {rms:F4}");

        if (rms < rmsThreshold)
        {
            Debug.Log($"[RayDebug] RMS {rms:F4} < Threshold {rmsThreshold:F4}, ���� �߻� ����");
            return;
        }

        if (ResourceCon.instance != null)
        {
            float drain = rms * soundDrainFactor * spawnInterval;
            ResourceCon.instance.Gage -= drain;
            Debug.Log($"[MicDecibelMeter] ResourceCon Gage -{drain:F2} �� {ResourceCon.instance.Gage:F2}");
        }

        if (wavePrefab != null)
        {
            float waveRadius = rms * waveScaleFactor;
            var centerWave = Instantiate(wavePrefab, spawnMarker.position, Quaternion.identity);
            if (centerWave.TryGetComponent<WaveRipple>(out var centerRipple))
                centerRipple.Initialize(waveRadius);
        }

        float rayLength = Mathf.Max(rms * rayLengthFactor, minRayLength);
        Vector2 origin2D = new Vector2(spawnMarker.position.x, spawnMarker.position.y);
        float stepDeg = 360f / rayCount;

        // LayerMask ��Ʈ ���� �����
        string maskBits = Convert.ToString(targetLayerMask.value, 2).PadLeft(32, '0');
        Debug.Log($"[RayDebug] LayerMask value: {targetLayerMask.value} (bits: {maskBits})");

        for (int i = 0; i < rayCount; i++)
        {
            float rad = stepDeg * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector2 rayOrigin = origin2D + dir * 0.05f;

            Debug.Log(
                $"[RayDebug] #{i}\n" +
                $"  Origin: {rayOrigin}\n" +
                $"  Dir: {dir} (angle: {stepDeg * i}��)\n" +
                $"  Length: {rayLength:F2}"
            );

            Debug.DrawRay(rayOrigin, dir * rayLength, Color.yellow, spawnInterval);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, dir, rayLength, targetLayerMask.value);

            if (hit.collider != null)
            {
                Debug.Log(
                    $"[RayDebug] �� HIT #{i}\n" +
                    $"    Collider Name: {hit.collider.name}\n" +
                    $"    Tag: {hit.collider.tag}\n" +
                    $"    IsTrigger: {hit.collider.isTrigger}\n" +
                    $"    Distance: {hit.distance:F2}\n" +
                    $"    Hit Point: {hit.point}\n" +
                    $"    Normal: {hit.normal}"
                );

                var lightGo = _lightPool.Get();
                lightGo.transform.SetParent(hit.collider.transform, worldPositionStays: true);
                lightGo.transform.position = hit.point;
                if (lightGo.TryGetComponent<Light2DFade>(out var fade))
                    fade.Initialize();
            }
            else
            {
                Debug.Log($"[RayDebug] �� MISS #{i} (no collider within {rayLength:F2})");
            }
        }
    }

    private float GetRMS()
    {
        int pos = Microphone.GetPosition(micDevice) - sampleWindow + 1;
        if (pos < 0) return 0f;
        _audioSource.clip.GetData(_samples, pos);
        float sum = 0f;
        foreach (var s in _samples) sum += s * s;
        return Mathf.Sqrt(sum / _samples.Length);
    }

    private void ReleaseLight(GameObject go)
    {
        _lightPool.Release(go);
    }
}
