using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class MicDecibelMeter : MonoBehaviour
{
    [Header("����ũ ����")]
    public string micDevice = "";
    public int sampleRate = 44100;
    public int sampleWindow = 1024;

    [Header("�ĵ� ȿ�� ����")]
    public GameObject wavePrefab;
    public float waveScaleFactor = 0.05f;
    public float spawnInterval = 0.2f;

    [Header("���� ��� ����Ʈ ���� ����")]
    [Tooltip("�� ���� ����")]
    public int rayCount = 16;
    [Tooltip("���� �ִ� ��Ÿ�")]
    public float rayDistance = 5f;
    [Tooltip("���� �� ���̾�� �浹 �� ����")]
    public LayerMask targetLayerMask;
    [Tooltip("�浹 ������ ������ ����Ʈ/��Į ������")]
    public GameObject lightPrefab;

    AudioSource audioSource;
    float[] samples;
    float spawnTimer = 0f;

    // �� ���⺰�� �� ���� �����ϵ��� �÷��� ����
    bool[] rayHasSpawned;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        samples = new float[sampleWindow];
        rayHasSpawned = new bool[rayCount];

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("����ũ ��ġ�� ã�� �� �����ϴ�.");
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
        // Shift ���� �ø��� ���� �÷��� �ʱ�ȭ
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

        // �ĵ� ���� (����)
        if (wavePrefab != null)
        {
            float maxScale = rms * waveScaleFactor;
            var go = Instantiate(wavePrefab, transform.position, Quaternion.identity);
            if (go.TryGetComponent<WaveRipple>(out var ripple))
                ripple.Initialize(maxScale);
        }

        // �� ���� ���⿡�� ù Map �浹 �������� ����Ʈ ����
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
            if (rayHasSpawned[i]) continue;  // �̹� ������ ������ �ǳʶٱ�

            float rad = (stepDeg * i) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            // ���� �浹 �˻� (Map ���̾)
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, rayDistance, targetLayerMask);

            // ����׿�: ���̸� �浹 ���������� �׸���
            if (hit.collider != null)
            {
                Debug.DrawLine(origin, hit.point, Color.yellow, spawnInterval);
                // �浹���� ������ ����
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
