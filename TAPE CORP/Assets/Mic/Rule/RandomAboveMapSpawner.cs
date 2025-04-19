using UnityEngine;
using UnityEngine.AI;

public class RandomNavMeshSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    [Tooltip("������ ������")]
    public GameObject prefabToSpawn;

    [Header("Spawn Settings")]
    [Tooltip("�� ���� �� ���� ��������")]
    public int spawnCount = 1;
    [Tooltip("���� ���� (��)")]
    public float spawnInterval = 5f;
    [Tooltip("����Ʈ ������ ã�� ���� �ִ� �õ� Ƚ��")]
    public int maxAttemptsPerSpawn = 10;

    private NavMeshTriangulation _navTri;
    private Camera _cam;
    private int _triCount;

    void Start()
    {
        // NavMesh ������ �̸� �����α�
        _navTri = NavMesh.CalculateTriangulation();
        _triCount = _navTri.indices.Length / 3;
        Debug.Log($"[NavMeshSpawner] Triangles: {_triCount}, Vertices: {_navTri.vertices.Length}");
        if (_triCount == 0)
            Debug.LogWarning("NavMesh�� ��� �ֽ��ϴ�. ���� NavMesh�� ����ũ�ϼ���.");

        // ���� ī�޶� ĳ��
        _cam = Camera.main;
        if (_cam == null)
            Debug.LogError("���� ī�޶� �����ϴ�. �±װ� MainCamera�� �����Ǿ� �ִ��� Ȯ���ϼ���.");

        // �ݺ� ȣ��
        InvokeRepeating(nameof(SpawnOnNavMesh), 0f, spawnInterval);
        Debug.Log($"[NavMeshSpawner] Will spawn every {spawnInterval} seconds.");
    }

    /// <summary>
    /// NavMesh ���� ���� ���� �� ī�޶� �� �ȿ� �ִ� ���� ��� �����մϴ�.
    /// </summary>
    void SpawnOnNavMesh()
    {
        Debug.Log($"[NavMeshSpawner] Spawn cycle started at {Time.time:F2}, spawnCount: {spawnCount}");

        if (_triCount == 0 || _cam == null)
            return;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            bool found = false;

            Debug.Log($"[NavMeshSpawner] Spawning instance {i + 1}/{spawnCount}");
            int usedAttempt = 0;
            int chosenTriangle = -1;

            // �ִ� maxAttemptsPerSpawnȸ�� ��ġ�� �õ�
            for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
            {
                usedAttempt = attempt + 1;
                int triIndex = Random.Range(0, _triCount);
                int t = triIndex * 3;
                Vector3 a = _navTri.vertices[_navTri.indices[t]];
                Vector3 b = _navTri.vertices[_navTri.indices[t + 1]];
                Vector3 c = _navTri.vertices[_navTri.indices[t + 2]];

                // �ﰢ�� ���� ���� ���� ���
                spawnPos = RandomPointInTriangle(a, b, c);
                Vector3 vp = _cam.WorldToViewportPoint(spawnPos);

                Debug.Log($"  Attempt {usedAttempt}/{maxAttemptsPerSpawn} on triangle {triIndex}: a={a}, b={b}, c={c}, candidate={spawnPos}, viewport={vp}");

                if (vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f)
                {
                    found = true;
                    chosenTriangle = triIndex;
                    Debug.Log($"  Valid spawn found on attempt {usedAttempt} at triangle {chosenTriangle}, worldPos={spawnPos}");
                    break;
                }
            }

            if (found)
            {
                Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                Debug.Log($"[NavMeshSpawner] Prefab instantiated at {spawnPos} after {usedAttempt} attempts.");
            }
            else
            {
                Debug.LogWarning($"[NavMeshSpawner] ����Ʈ �� ��ȿ ���� ��ġ�� ã�� ���߽��ϴ� (�õ�: {maxAttemptsPerSpawn}ȸ). ������ �õ� ��ġ: {spawnPos}");
            }
        }
    }

    /// <summary>
    /// �ﰢ��(a, b, c) ���ο��� �յ� ������ ���� ��ȯ�մϴ�.
    /// </summary>
    private Vector3 RandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float r = Random.value;
        float s = Random.value;
        if (r + s > 1f)
        {
            r = 1f - r;
            s = 1f - s;
        }
        return a + r * (b - a) + s * (c - a);
    }
}
