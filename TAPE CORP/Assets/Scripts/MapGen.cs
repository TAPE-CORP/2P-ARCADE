using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FloatingIslandsGenerator : MonoBehaviour
{
    [Header("Tilemap 설정")]
    public Tilemap tilemap;
    public TileBase groundTile;       // 평탄한 바닥에 사용할 타일
    public TileBase islandTile;       // 섬(노드)에 사용할 타일

    [Header("맵/섬 설정")]
    public Vector2Int mapSize = new Vector2Int(100, 100); // 전체 맵 크기

    [Header("플레이어 점프 관련")]
    public float playerJumpDistance = 5f; // 최대 수평 점프 가능 거리
    public float playerJumpHeight = 3f;   // 최대 수직 점프 가능 거리
    [Header("플레이어 리스트")]
    public List<Transform> players;

    [Header("섬 설정")]
    public float minIslandRadius = 3f; // 섬의 최소 반경
    public float maxIslandRadius = 8f; // 섬의 최대 반경

    [Header("Perlin 노이즈")]
    public float noiseScale = 0.1f;   // 노이즈 스케일 (값이 클수록 노이즈 변화가 큼)

    [Header("오브젝트 프리팹 (선택)")]
    public GameObject monsterPrefab;
    public GameObject itemPrefab;
    public GameObject lightPrefab;
    public GameObject boxPrefab;
    public GameObject doorPrefab;
    public GameObject groundObject;
    public Transform spawnParent;

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

        tilemap.RefreshAllTiles();
        Debug.Log("월드 생성 완료!");
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
        return worldPosition; // 못 찾았으면 원래 위치
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
        Debug.Log("공중 섬 생성 중...");

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
                            if (!(IsSurrounded(x, y))) // 아이템이 들어갈 공간 보장
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), islandTile);
                                tilePlaced[x - minX, y - minY] = true;

                                // 천장이 생겼고, 아래에 땅이 있다면 라이트 설치
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
        // 땅 위로 플레이어 올리기
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

        // 박스도 모두 올리기
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

        Debug.Log("공중 섬 생성 완!");
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
        return solidCount >= 4; // 사방이 막힌 경우 true
    }
}