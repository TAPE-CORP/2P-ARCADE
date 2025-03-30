using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

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
    [Header("�÷��̾� ����Ʈ")]
    public List<Transform> players;

    [Header("�� ����")]
    public float minIslandRadius = 3f; // ���� �ּ� �ݰ�
    public float maxIslandRadius = 8f; // ���� �ִ� �ݰ�

    [Header("Perlin ������")]
    public float noiseScale = 0.1f;   // ������ ������ (���� Ŭ���� ������ ��ȭ�� ŭ)

    [Header("������Ʈ ������ (����)")]
    public GameObject monsterPrefab;
    public GameObject itemPrefab;
    public GameObject lightPrefab;
    public GameObject boxPrefab;
    public GameObject doorPrefab;
    public GameObject groundObject;
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

        GenerateGround();
        GenerateIslands();

        tilemap.RefreshAllTiles();
        Debug.Log("���� ���� �Ϸ�!");
    }
    private Vector3 SnapAboveGround(Vector3 worldPosition)
    {
        Vector3Int cell = tilemap.WorldToCell(worldPosition);
        for (int y = cell.y; y < mapSize.y; y++)
        {
            if (tilemap.HasTile(new Vector3Int(cell.x, y, 0)))
            {
                return tilemap.CellToWorld(new Vector3Int(cell.x, y + 1, 0)) + new Vector3(0.5f, 0.5f, 0);
            }
        }
        return worldPosition; // �� ã������ ���� ��ġ
    }

    private void GenerateGround()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            tilemap.SetTile(new Vector3Int(x, 0, 0), groundTile);
        }

        if (groundObject)
        {
            Vector3Int groundCell = new Vector3Int(mapSize.x - 1, 1, 0);
            Vector3 spawnPos = tilemap.CellToWorld(groundCell) + new Vector3(0.5f, 0.5f, 0);
            Instantiate(groundObject, spawnPos, Quaternion.identity, spawnParent);
        }
    }

    private void GenerateIslands()
    {
        Debug.Log("���� �� ���� ��...");

        float currentX = 0;
        float lastCenterY = 0;
        float minIslandY = 1;

        while (currentX < mapSize.x)
        {
            float radius = Random.Range(minIslandRadius, maxIslandRadius);
            float centerX = currentX + radius;
            float deltaY = Random.Range(-playerJumpHeight, playerJumpHeight);
            float centerY = Mathf.Clamp(lastCenterY + deltaY, minIslandY, mapSize.y - 1);
            Vector2 center = new Vector2(centerX, centerY);

            int minX = Mathf.FloorToInt(centerX - radius);
            int maxX = Mathf.CeilToInt(centerX + radius);
            int minY = Mathf.FloorToInt(centerY - radius);
            int maxY = Mathf.CeilToInt(centerY + radius);

            bool[,] tilePlaced = new bool[maxX - minX + 1, maxY - minY + 1];

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
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
                            if (!(IsSurrounded(x, y))) // �������� �� ���� ����
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), islandTile);
                                tilePlaced[x - minX, y - minY] = true;

                                // õ���� �����, �Ʒ��� ���� �ִٸ� ����Ʈ ��ġ
                                if (y < mapSize.y - 1 && tilemap.HasTile(new Vector3Int(x, y - 1, 0)))
                                {
                                    int horizontalClear = 0;
                                    for (int dxL = -2; dxL <= 2; dxL++)
                                    {
                                        if (tilemap.HasTile(new Vector3Int(x + dxL, y, 0)))
                                            horizontalClear++;
                                    }
                                    if (horizontalClear >= 5 && lightPrefab != null)
                                    {
                                        Vector3 lightPos = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, 0);
                                        Instantiate(lightPrefab, lightPos, Quaternion.identity, spawnParent);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Vector3Int islandCenterCell = new Vector3Int(Mathf.RoundToInt(centerX), Mathf.RoundToInt(centerY), 0);
            Vector3 spawnPosCenter = tilemap.CellToWorld(islandCenterCell) + new Vector3(0.5f, 0.5f, 0);
            if (itemPrefab)
                Instantiate(itemPrefab, spawnPosCenter, Quaternion.identity, spawnParent);

            if (monsterPrefab && Random.value < 0.5f)
            {
                Vector3 spawnPosMonster = spawnPosCenter + new Vector3(-0.3f, 0.3f, 0f);
                Instantiate(monsterPrefab, spawnPosMonster, Quaternion.identity, spawnParent);
            }

            currentX = centerX + radius + playerJumpDistance * Random.Range(0.8f, 1.3f);
            lastCenterY = centerY;
        }

        if (boxPrefab)
        {
            Vector3 boxPos = tilemap.CellToWorld(new Vector3Int(mapSize.x / 2, 4, 0)) + new Vector3(0.5f, 0.5f, 0);
            Instantiate(boxPrefab, boxPos, Quaternion.identity, spawnParent);
        }

        if (doorPrefab)
        {
            Vector3 doorPos = tilemap.CellToWorld(new Vector3Int(mapSize.x / 2 + 2, 4, 0)) + new Vector3(0.5f, 0.5f, 0);
            Instantiate(doorPrefab, doorPos, Quaternion.identity, spawnParent);
        }
        // �� ���� �÷��̾� �ø���
        if (players != null)
        {
            foreach (var player in players)
            {
                if (player != null)
                {
                    player.position = SnapAboveGround(player.position);
                }
            }
        }

        // �ڽ��� ��� �ø���
        if (boxPrefab)
        {
            foreach (Transform child in spawnParent)
            {
                if (child.CompareTag("Box"))
                {
                    child.position = SnapAboveGround(child.position);
                }
            }
        }

        Debug.Log("���� �� ���� ��!");
    }

    private bool IsSurrounded(int x, int y)
    {
        int solidCount = 0;
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0)
        };
        foreach (var dir in directions)
        {
            if (tilemap.HasTile(new Vector3Int(x, y, 0) + dir))
                solidCount++;
        }
        return solidCount >= 4; // ����� ���� ��� true
    }
}