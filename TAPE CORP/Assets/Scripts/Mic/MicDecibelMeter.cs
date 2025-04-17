using UnityEngine;
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
    public float waveScaleFactor = 0.05f;
    public float spawnInterval = 0.2f;

    [Header("레이 기반 라이트 스폰 설정")]
    [Tooltip("쏠 레이 개수")]
    public int rayCount = 16;
    [Tooltip("레이 최대 사거리")]
    public float rayDistance = 5f;
    [Tooltip("오직 이 레이어와 충돌 시 반응")]
    public LayerMask targetLayerMask;
    [Tooltip("충돌 지점에 생성할 라이트/데칼 프리팹")]
    public GameObject lightPrefab;

    AudioSource audioSource;
    float[] samples;
    float spawnTimer = 0f;

    // 각 방향별로 한 번만 스폰하도록 플래그 관리
    bool[] rayHasSpawned;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        samples = new float[sampleWindow];
        rayHasSpawned = new bool[rayCount];

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("마이크 장치를 찾을 수 없습니다.");
            enabled = false;
            return;
        }
        if (string.IsNullOrEmpty(micDevice))
            micDevice = Microphone.devices[0];

        audioSource.clip = Microphone.Start(micDevice, true, 1, sampleRate);
        audioSource.loop = true;
        audioSource.mute = true;
        while (Microphone.GetPosition(micDevice) <= 0) { }
        audioSource.Play();
    }

    void Update()
    {
        // Shift 시작 시마다 스폰 플래그 초기화
        if (Input.GetKeyDown(KeyCode.LeftShift))
            System.Array.Clear(rayHasSpawned, 0, rayHasSpawned.Length);

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            spawnTimer = spawnInterval;
            return;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer < spawnInterval) return;
        spawnTimer = 0f;

        float rms = GetRMS();
        float db = 20f * Mathf.Log10(rms / 0.1f);
        if (db < -80f) db = -80f;

        // 파동 생성 (선택)
        if (wavePrefab != null)
        {
            float maxScale = rms * waveScaleFactor;
            var go = Instantiate(wavePrefab, transform.position, Quaternion.identity);
            if (go.TryGetComponent<WaveRipple>(out var ripple))
                ripple.Initialize(maxScale);
        }

        // 각 레이 방향에서 첫 Map 충돌 지점에만 라이트 생성
        SpawnLightPerRay();
    }

    float GetRMS()
    {
        int pos = Microphone.GetPosition(micDevice) - sampleWindow + 1;
        if (pos < 0) return 0f;
        audioSource.clip.GetData(samples, pos);
        float sum = 0f;
        foreach (var s in samples) sum += s * s;
        return Mathf.Sqrt(sum / samples.Length);
    }

    void SpawnLightPerRay()
    {
        if (lightPrefab == null) return;

        Vector2 origin = transform.position;
        float stepDeg = 360f / rayCount;

        for (int i = 0; i < rayCount; i++)
        {
            if (rayHasSpawned[i]) continue;  // 이미 스폰된 방향은 건너뛰기

            float rad = (stepDeg * i) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            // 실제 충돌 검사 (Map 레이어만)
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, rayDistance, targetLayerMask);

            // 디버그용: 레이를 충돌 지점까지만 그리기
            if (hit.collider != null)
            {
                Debug.DrawLine(origin, hit.point, Color.yellow, spawnInterval);
                // 충돌점에 프리팹 생성
                Vector3 pos = new Vector3(hit.point.x, hit.point.y, hit.collider.transform.position.z - 0.01f);
                Instantiate(lightPrefab, pos, Quaternion.identity, hit.collider.transform);
                rayHasSpawned[i] = true;
            }
            else
            {
                Debug.DrawLine(origin, origin + dir * rayDistance, Color.grey, spawnInterval);
            }
        }
    }
}
