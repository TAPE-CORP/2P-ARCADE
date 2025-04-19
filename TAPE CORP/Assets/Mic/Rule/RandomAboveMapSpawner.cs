using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider2D))]
public class RandomNavMeshSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    [Tooltip("스폰할 프리팹")]
    public GameObject prefabToSpawn;

    [Header("Spawn Settings")]
    [Tooltip("한 번에 몇 개를 스폰할지")]
    public int spawnCount = 1;
    [Tooltip("스폰 간격 (초)")]
    public float spawnInterval = 5f;
    [Tooltip("뷰포트 내에서 찾기 위한 최대 시도 횟수")]
    public int maxAttemptsPerSpawn = 10;

    private NavMeshTriangulation _navTri;
    private Camera _cam;
    private int _triCount;

    void Start()
    {
        // NavMesh 데이터 미리 가져두기
        _navTri = NavMesh.CalculateTriangulation();
        _triCount = _navTri.indices.Length / 3;
        if (_triCount == 0)
            Debug.LogWarning("NavMesh가 비어 있습니다. 먼저 NavMesh를 베이크하세요.");

        // 메인 카메라 캐시
        _cam = Camera.main;
        if (_cam == null)
            Debug.LogError("메인 카메라가 없습니다. 태그가 MainCamera로 설정되어 있는지 확인하세요.");

        // 반복 호출
        InvokeRepeating(nameof(SpawnOnNavMesh), 0f, spawnInterval);
    }

    /// <summary>
    /// NavMesh 위의 임의 지점 중 카메라 뷰 안에 있는 곳을 골라 스폰합니다.
    /// </summary>
    void SpawnOnNavMesh()
    {
        if (_triCount == 0 || _cam == null)
            return;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            bool found = false;

            // 최대 maxAttemptsPerSpawn회만 위치를 시도
            for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
            {
                // 랜덤 삼각형 선택
                int t = Random.Range(0, _triCount) * 3;
                Vector3 a = _navTri.vertices[_navTri.indices[t]];
                Vector3 b = _navTri.vertices[_navTri.indices[t + 1]];
                Vector3 c = _navTri.vertices[_navTri.indices[t + 2]];

                // 삼각형 내부 랜덤 지점 계산
                spawnPos = RandomPointInTriangle(a, b, c);

                // 뷰포트 좌표 확인
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
                Debug.LogWarning($"뷰포트 내 유효 스폰 위치를 찾지 못했습니다 (시도: {maxAttemptsPerSpawn}회).");
            }
        }
    }

    /// <summary>
    /// 삼각형(a, b, c) 내부에서 균등 분포한 점을 반환합니다.
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
