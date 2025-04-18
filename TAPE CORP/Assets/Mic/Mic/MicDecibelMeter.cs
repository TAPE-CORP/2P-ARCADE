using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class MicDecibelMeter : MonoBehaviour
{
    [Header("마이크 설정")]
    public string micDevice = "";
    public int sampleRate = 44100;
    public int sampleWindow = 1024;

    [Header("파동 효과 설정")]
    public GameObject wavePrefab;
    [Tooltip("RMS * 이 값 = 파동 최대 반경")]
    public float waveScaleFactor = 0.05f;
    public float spawnInterval = 0.2f;

    [Header("레이 길이 설정")]
    [Tooltip("RMS * 이 값 = 레이 길이")]
    public float rayLengthFactor = 5f;
    public int rayCount = 16;
    [Tooltip("충돌 검사할 벽 레이어")]
    public LayerMask targetLayerMask;

    [Header("라이트 풀 설정")]
    public GameObject lightPrefab;
    public int poolCapacity = 32;

    [Header("레이 발사 기준 트랜스폼")]
    public Transform spawnMarker;

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
                go.transform.SetParent(null);  // <- 여기에서 부모 해제 (중요!)
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
            Debug.LogError("마이크 장치를 찾을 수 없습니다.");
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
        float waveRadius = rms * waveScaleFactor;
        float rayLength = rms * rayLengthFactor;

        // 중앙 파동 효과 생성
        if (wavePrefab != null)
        {
            var centerWave = Instantiate(wavePrefab, spawnMarker.position, Quaternion.identity);
            if (centerWave.TryGetComponent<WaveRipple>(out var centerRipple))
                centerRipple.Initialize(waveRadius);
        }

        // 레이 발사 후 충돌 시 라이트 활성화
        Vector2 origin = spawnMarker.position;
        float stepDeg = 360f / rayCount;

        for (int i = 0; i < rayCount; i++)
        {
            float ang = stepDeg * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));

            Debug.DrawRay(origin, dir * rayLength, Color.yellow, spawnInterval);

            var hit = Physics2D.Raycast(origin, dir, rayLength, targetLayerMask);
            if (hit.collider != null)
            {
                var lightGo = _lightPool.Get();
                lightGo.transform.SetParent(hit.collider.transform, worldPositionStays: true);
                lightGo.transform.position = hit.point;

                // 라이트 초기화 호출 (핵심 추가 부분)
                if (lightGo.TryGetComponent<Light2DFade>(out var fade))
                    fade.Initialize();
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
