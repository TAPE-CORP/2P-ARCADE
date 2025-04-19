using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider2D))]
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
        if (_triCount == 0)
            Debug.LogWarning("NavMesh�� ��� �ֽ��ϴ�. ���� NavMesh�� ����ũ�ϼ���.");

        // ���� ī�޶� ĳ��
        _cam = Camera.main;
        if (_cam == null)
            Debug.LogError("���� ī�޶� �����ϴ�. �±װ� MainCamera�� �����Ǿ� �ִ��� Ȯ���ϼ���.");

        // �ݺ� ȣ��
        InvokeRepeating(nameof(SpawnOnNavMesh), 0f, spawnInterval);
    }

    /// <summary>
    /// NavMesh ���� ���� ���� �� ī�޶� �� �ȿ� �ִ� ���� ��� �����մϴ�.
    /// </summary>
    void SpawnOnNavMesh()
    {
        if (_triCount == 0 || _cam == null)
            return;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            bool found = false;

            // �ִ� maxAttemptsPerSpawnȸ�� ��ġ�� �õ�
            for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
            {
                // ���� �ﰢ�� ����
                int t = Random.Range(0, _triCount) * 3;
                Vector3 a = _navTri.vertices[_navTri.indices[t]];
                Vector3 b = _navTri.vertices[_navTri.indices[t + 1]];
                Vector3 c = _navTri.vertices[_navTri.indices[t + 2]];

                // �ﰢ�� ���� ���� ���� ���
                spawnPos = RandomPointInTriangle(a, b, c);

                // ����Ʈ ��ǥ Ȯ��
                Vector3 vp = _cam.WorldToViewportPoint(spawnPos);
                if (vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"����Ʈ �� ��ȿ ���� ��ġ�� ã�� ���߽��ϴ� (�õ�: {maxAttemptsPerSpawn}ȸ).");
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
