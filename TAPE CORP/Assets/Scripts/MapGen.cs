using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloatingIslandsGenerator : MonoBehaviour
{
    [Header("Tilemap 설정")]
    public Tilemap tilemap;
    public TileBase groundTile;
    public TileBase islandTile;

    [Header("맵/섬 설정")]
    public Vector2Int mapSize = new Vector2Int(100, 100);

    [Header("플레이어 점프 관련")]
    public float playerJumpDistance = 5f;
    public float playerJumpHeight = 3f;

    [Header("섬 설정")]
    public float minIslandRadius = 3f;
    public float maxIslandRadius = 8f;

    [Header("Perlin 노이즈")]
    public float noiseScale = 0.1f;

    [Header("오브젝트 프리팹")]
    public GameObject monsterPrefab;
    public GameObject itemPrefab;
    public GameObject lightPrefab;
    public GameObject boxPrefab;
    public GameObject groundObject;
    public Transform spawnParent;

    [Header("문 컴포넌트 (1개만 존재)")]
    public Door targetDoor;

    [Header("플레이어 시작 위치 리스트")]
    public List<Transform> playerStarts;

    [Header("랜덤 시드")]
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
        Debug.Log("월드 생성 시작...");
        GenerateGround();
        GenerateIslands();
        GenerateBoxAndDoor();
        tilemap.RefreshAllTiles();
        Debug.Log("월드 생성 완료!");
    }

    private void GenerateGround()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            tilemap.SetTile(new Vector3Int(x, 0, 0), groundTile);
        }

        if (groundObject && playerStarts != null)
        {
            foreach (var player in playerStarts)
            {
                if (player == null) continue;

                Vector3Int cellPos = tilemap.WorldToCell(player.position + Vector3.down);
                Vector3 spawnPos = tilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0);
                Instantiate(groundObject, spawnPos, Quaternion.identity, spawnParent);
            }
        }
    }

    private void GenerateIslands()
    {
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

                        if (noiseVal > dynamicThreshold && !IsSurrounded(x, y))
                        {
                            tilemap.SetTile(new Vector3Int(x, y, 0), islandTile);

                            if (y < mapSize.y - 1 && tilemap.HasTile(new Vector3Int(x, y - 1, 0)))
                            {
                                int horizontal = 0;
                                for (int dxL = -2; dxL <= 2; dxL++)
                                {
                                    if (tilemap.HasTile(new Vector3Int(x + dxL, y, 0)))
                                        horizontal++;
                                }

                                if (horizontal >= 5 && lightPrefab != null)
                                {
                                    Vector3 lightPos = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, 0);
                                    GameObject light = Instantiate(lightPrefab, lightPos, Quaternion.identity, spawnParent);

                                    Light template = lightPrefab.GetComponent<Light>();
                                    Light inst = light.GetComponent<Light>();
                                    if (template && inst)
                                    {
                                        inst.type = template.type;
                                        inst.color = template.color;
                                        inst.range = template.range;
                                        inst.intensity = template.intensity;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Vector3Int centerCell = new Vector3Int(Mathf.RoundToInt(centerX), Mathf.RoundToInt(centerY), 0);
            Vector3 spawnCenter = tilemap.CellToWorld(centerCell) + new Vector3(0.5f, 0.5f, 0);
            if (itemPrefab)
                Instantiate(itemPrefab, spawnCenter, Quaternion.identity, spawnParent);
            if (monsterPrefab && Random.value < 0.5f)
                Instantiate(monsterPrefab, spawnCenter + new Vector3(-0.3f, 0.3f, 0), Quaternion.identity, spawnParent);

            currentX = centerX + radius + playerJumpDistance * Random.Range(0.8f, 1.3f);
            lastCenterY = centerY;
        }
    }

    private void GenerateBoxAndDoor()
    {
        bool boxPlaced = false;
        for (int y = 1; y < mapSize.y; y++)
        {
            for (int x = 0; x <= mapSize.x - 10; x++)
            {
                bool isFlat = true;
                for (int i = 0; i < 10; i++)
                {
                    if (!tilemap.HasTile(new Vector3Int(x + i, y, 0)))
                    {
                        isFlat = false;
                        break;
                    }
                }

                if (isFlat)
                {
                    if (!boxPlaced && boxPrefab)
                    {
                        Vector3 boxPos = tilemap.CellToWorld(new Vector3Int(x + 5, y + 1, 0)) + new Vector3(0.5f, 0.5f, 0);
                        GameObject box = Instantiate(boxPrefab, boxPos, Quaternion.identity, spawnParent);
                        box.AddComponent<GrabbableBox>().targetDoor = targetDoor;
                        boxPlaced = true;
                    }

                    if (targetDoor)
                    {
                        Vector3 doorPos = tilemap.CellToWorld(new Vector3Int(x + 8, y + 1, 0)) + new Vector3(0.5f, 0.5f, 0);
                        targetDoor.transform.position = doorPos;
                    }

                    return;
                }
            }
        }
    }

    private bool IsSurrounded(int x, int y)
    {
        int solid = 0;
        Vector3Int[] dirs = {
            Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down
        };
        foreach (var d in dirs)
        {
            if (tilemap.HasTile(new Vector3Int(x, y, 0) + d)) solid++;
        }
        return solid >= 4;
    }
}
