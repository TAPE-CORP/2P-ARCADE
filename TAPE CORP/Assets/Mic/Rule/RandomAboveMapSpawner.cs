using UnityEngine;

public class RandomAboveMapSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    public GameObject prefabToSpawn;

    [Header("Spawn Settings")]
    [Tooltip("�� ���� �� ���� ��������")]
    public int spawnCount = 1;
    [Tooltip("Map �±� ������Ʈ �������� ��� �ּ� Y ������")]
    public float minYOffset = 1f;
    [Tooltip("Map �±� ������Ʈ �������� ��� �ִ� Y ������")]
    public float maxYOffset = 3f;
    [Tooltip("���� ���� (��)")]
    public float spawnInterval = 5f;

    void Start()
    {
        // 0�� �� ù ����, ���� spawnInterval���� �ݺ� ȣ��
        InvokeRepeating(nameof(SpawnAboveRandomMapObjects), 0f, spawnInterval);
    }

    /// <summary>
    /// "Map" �±װ� ���� ������Ʈ �� �������� ���,
    /// �� ������Ʈ�� Renderer.bounds ������ ���� ������ prefab�� �����մϴ�.
    /// </summary>
    public void SpawnAboveRandomMapObjects()
    {
        var maps = GameObject.FindGameObjectsWithTag("Map");
        if (maps.Length == 0)
        {
            Debug.LogWarning("Map �±װ� ���� ������Ʈ�� �����ϴ�.");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            // 1) ���� Map ������Ʈ ����
            var target = maps[Random.Range(0, maps.Length)];
            var rend = target.GetComponent<Renderer>();
            if (rend == null)
            {
                Debug.LogWarning($"{target.name}�� Renderer�� ���� ��ġ ����� �ǳʶݴϴ�.");
                continue;
            }

            // 2) Renderer.bounds�� �̿��� ���� ������ ��� y ���
            Bounds b = rend.bounds;
            float x = Random.Range(b.min.x, b.max.x);
            float y = b.max.y + Random.Range(minYOffset, maxYOffset);

            // 3) ����
            Instantiate(prefabToSpawn, new Vector3(x, y, target.transform.position.z), Quaternion.identity);
        }
    }
}
