using UnityEngine;
using UnityEngine.Tilemaps;

public class FloatingIslandsGenerator : MonoBehaviour
{
    [Header("Tilemap ����")]
    public Tilemap tilemap;
    public TileBase groundTile;       // ��ź�� �ٴڿ� ����� Ÿ��
    public TileBase islandTile;       // ��(���)�� ����� Ÿ��

    [Header("��/�� ����")]
    public Vector2Int mapSize = new Vector2Int(100, 100); // ��ü �� ũ��

    [Header("�÷��̾� ���� ����")]
    public float playerJumpDistance = 5f; // �ִ� ���� ���� ���� �Ÿ�
    public float playerJumpHeight = 3f;   // �ִ� ���� ���� ���� �Ÿ�

    [Header("�� ����")]
    public float minIslandRadius = 3f; // ���� �ּ� �ݰ�
    public float maxIslandRadius = 8f; // ���� �ִ� �ݰ�

    [Header("Perlin ������")]
    public float noiseScale = 0.1f;   // ������ ������ (���� Ŭ���� ������ ��ȭ�� ŭ)

    [Header("������Ʈ ������ (����)")]
    public GameObject monsterPrefab;
    public GameObject itemPrefab;
    public Transform spawnParent;

    [Header("���� �õ�")]
    public bool useRandomSeed = true;
    public int seed = 0;

    private void Awake()
    {
        if (useRandomSeed)
            seed = Random.Range(0, 1000000);

        GenerateWorld();
    }

    private void GenerateWorld()
    {
        Debug.Log("���� ���� ����...");

        // 1. ��ź�� �ٴ� ���� (y��ǥ 0)
        GenerateGround();

        // 2. ĳ������ �������� �ݿ��Ͽ� ���� ��(���) ���� (����, ���� ��� ���)
        GenerateIslands();

        tilemap.RefreshAllTiles();
        Debug.Log("���� ���� �Ϸ�!");
    }

    /// <summary>
    /// ���� y��ǥ 0�� ��ź�� �ٴ� Ÿ���� ��ü ���η� �����մϴ�.
    /// </summary>
    private void GenerateGround()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            tilemap.SetTile(new Vector3Int(x, 0, 0), groundTile);
        }
    }

    /// <summary>
    /// ĳ������ ���� �� ���� �������� �����, ���߿� ��(���)�� ���������� �����մϴ�.
    /// ���� �������� �ٴ�(y=0)���� �Ͽ�, ù ������ ĳ���Ͱ� ������ �� �ֵ��� �մϴ�.
    /// </summary>
    private void GenerateIslands()
    {
        Debug.Log("���� �� ���� ��...");

        float currentX = 0;
        // ���� �������� �ٴ����� ���� (y=0)
        float lastCenterY = 0;
        // ���� �ٴ� �ٷ� ������ �����ǹǷ� �ּ� y���� 1�� ����
        float minIslandY = 1;

        while (currentX < mapSize.x)
        {
            // ���� �ݰ� ����
            float radius = Random.Range(minIslandRadius, maxIslandRadius);

            // ���� �߽� ��ǥ: x�� currentX + radius,
            // y�� ���� �� �߽ɿ��� [-playerJumpHeight, playerJumpHeight] ���� ������ �����ϸ�, �ּ� 1 �̻��� ���̷� Ŭ����
            float centerX = currentX + radius;
            float deltaY = Random.Range(-playerJumpHeight, playerJumpHeight);
            float centerY = Mathf.Clamp(lastCenterY + deltaY, minIslandY, mapSize.y - 1);
            Vector2 center = new Vector2(centerX, centerY);

            // ���� ���� ���
            int minX = Mathf.FloorToInt(centerX - radius);
            int maxX = Mathf.CeilToInt(centerX + radius);
            int minY = Mathf.FloorToInt(centerY - radius);
            int maxY = Mathf.CeilToInt(centerY + radius);

            // ���� �� �� ��ǥ�� ���� �� Ÿ�� ���� ���� ���� (�� ���� ���� + Perlin ������� �ڿ������� ���)
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    // �� ���� üũ
                    if (x < 0 || x >= mapSize.x || y < 0 || y >= mapSize.y)
                        continue;

                    float dx = (x - centerX) / radius;
                    float dy = (y - centerY) / radius;
                    float dist = dx * dx + dy * dy;
                    if (dist <= 1f)
                    {
                        float noiseVal = Mathf.PerlinNoise((x + seed) * noiseScale, (y + seed) * noiseScale);
                        float threshold = 0.5f;
                        float dynamicThreshold = threshold + dist * 0.2f;
                        if (noiseVal > dynamicThreshold)
                        {
                            tilemap.SetTile(new Vector3Int(x, y, 0), islandTile);
                        }
                    }
                }
            }

            // ���� ����: �� �߾ӿ� ������ �� ���� ����
            Vector3Int islandCenterCell = new Vector3Int(Mathf.RoundToInt(centerX), Mathf.RoundToInt(centerY), 0);
            Vector3 spawnPosCenter = tilemap.CellToWorld(islandCenterCell) + new Vector3(0.5f, 0.5f, 0);
            if (itemPrefab)
            {
                Instantiate(itemPrefab, spawnPosCenter, Quaternion.identity, spawnParent);
            }
            if (monsterPrefab && Random.value < 0.5f)
            {
                Vector3 spawnPosMonster = spawnPosCenter + new Vector3(-0.3f, 0.3f, 0f);
                Instantiate(monsterPrefab, spawnPosMonster, Quaternion.identity, spawnParent);
            }

            // ���� �������� ���� ������ ���� ���� ��(2 * radius)�� playerJumpDistance�� �ݿ��մϴ�.
            float jumpGap = playerJumpDistance * Random.Range(0.8f, 1.3f);
            currentX = centerX + radius + jumpGap;
            // ���� ���� ���� ������ ����
            lastCenterY = centerY;
        }

        Debug.Log("���� �� ���� �Ϸ�!");
    }
}
