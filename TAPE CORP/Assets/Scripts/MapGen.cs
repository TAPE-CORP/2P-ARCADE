using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FloatingIslandsGenerator : MonoBehaviour
{
    [Header("맵/섬 설정")]
    public Vector2Int mapSize = new Vector2Int(100, 100);
    public Vector2Int mapOffset = new Vector2Int(0, 0);

    [Header("Tilemap 설정")]
    public Tilemap tilemap;
    public TileBase groundTile;
    public TileBase islandTile;

    [Header("플레이어 점프 관련")]
    public float playerJumpDistance = 5f;
    public float playerJumpHeight = 3f;

    [Header("플레이어 리스트")]
    public List<Transform> players;

    [Header("섬 설정")]
    public float minIslandRadius = 3f;
    public float maxIslandRadius = 8f;

    [Header("Perlin 노이즈")]
    public float noiseScale = 0.1f;

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
        //GenerateGround();
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
        return worldPosition;
    }
    /// <summary>
    /// 디버깅용 바닥 생성
    /// </summary>
    private void GenerateGround()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            tilemap.SetTile(new Vector3Int(x + mapOffset.x, mapOffset.y, 0), groundTile);
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
            float centerX = currentX + radius + mapOffset.x;
            float deltaY = Random.Range(-playerJumpHeight, playerJumpHeight);
            float centerY = Mathf.Clamp(lastCenterY + deltaY, minIslandY, mapSize.y - 1) + mapOffset.y;
            Vector2 center = new Vector2(centerX, centerY);

            int minX = Mathf.FloorToInt(centerX - radius);
            int maxX = Mathf.CeilToInt(centerX + radius);
            int minY = Mathf.FloorToInt(centerY - radius);
            int maxY = Mathf.CeilToInt(centerY + radius);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (x < mapOffset.x || x >= mapOffset.x + mapSize.x || y < mapOffset.y || y >= mapOffset.y + mapSize.y)
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
                            if (!IsSurrounded(x, y))
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), islandTile);
                            }
                        }
                    }
                }
            }

            currentX = centerX - mapOffset.x + radius + playerJumpDistance * Random.Range(0.8f, 1.3f);
            lastCenterY = centerY - mapOffset.y;
        }

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
        return solidCount >= 4;
    }
}
