using UnityEngine;

public class RandomAboveMapSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    public GameObject prefabToSpawn;

    [Header("Spawn Settings")]
    [Tooltip("한 번에 몇 개를 스폰할지")]
    public int spawnCount = 1;
    [Tooltip("Map 태그 오브젝트 위쪽으로 띄울 최소 Y 오프셋")]
    public float minYOffset = 1f;
    [Tooltip("Map 태그 오브젝트 위쪽으로 띄울 최대 Y 오프셋")]
    public float maxYOffset = 3f;
    [Tooltip("스폰 간격 (초)")]
    public float spawnInterval = 5f;

    void Start()
    {
        // 0초 후 첫 스폰, 이후 spawnInterval마다 반복 호출
        InvokeRepeating(nameof(SpawnAboveRandomMapObjects), 0f, spawnInterval);
    }

    /// <summary>
    /// "Map" 태그가 붙은 오브젝트 중 랜덤으로 골라,
    /// 그 오브젝트의 Renderer.bounds 위쪽의 랜덤 지점에 prefab을 생성합니다.
    /// </summary>
    public void SpawnAboveRandomMapObjects()
    {
        var maps = GameObject.FindGameObjectsWithTag("Map");
        if (maps.Length == 0)
        {
            Debug.LogWarning("Map 태그가 붙은 오브젝트가 없습니다.");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            // 1) 랜덤 Map 오브젝트 선택
            var target = maps[Random.Range(0, maps.Length)];
            var rend = target.GetComponent<Renderer>();
            if (rend == null)
            {
                Debug.LogWarning($"{target.name}에 Renderer가 없어 위치 계산을 건너뜁니다.");
                continue;
            }

            // 2) Renderer.bounds를 이용해 가로 범위와 상단 y 계산
            Bounds b = rend.bounds;
            float x = Random.Range(b.min.x, b.max.x);
            float y = b.max.y + Random.Range(minYOffset, maxYOffset);

            // 3) 스폰
            Instantiate(prefabToSpawn, new Vector3(x, y, target.transform.position.z), Quaternion.identity);
        }
    }
}
