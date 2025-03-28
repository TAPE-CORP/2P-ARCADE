using UnityEngine;
using UnityEngine.Tilemaps;

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

    [Header("섬 설정")]
    public float minIslandRadius = 3f; // 섬의 최소 반경
    public float maxIslandRadius = 8f; // 섬의 최대 반경

    [Header("Perlin 노이즈")]
    public float noiseScale = 0.1f;   // 노이즈 스케일 (값이 클수록 노이즈 변화가 큼)

    [Header("오브젝트 프리팹 (선택)")]
    public GameObject monsterPrefab;
    public GameObject itemPrefab;
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

        // 1. 평탄한 바닥 생성 (y좌표 0)
        GenerateGround();

        // 2. 캐릭터의 점프력을 반영하여 공중 섬(노드) 생성 (수평, 수직 모두 고려)
        GenerateIslands();

        tilemap.RefreshAllTiles();
        Debug.Log("월드 생성 완료!");
    }

    /// <summary>
    /// 맵의 y좌표 0에 평탄한 바닥 타일을 전체 가로로 생성합니다.
    /// </summary>
    private void GenerateGround()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            tilemap.SetTile(new Vector3Int(x, 0, 0), groundTile);
        }
    }

    /// <summary>
    /// 캐릭터의 수평 및 수직 점프력을 고려해, 공중에 섬(노드)을 순차적으로 생성합니다.
    /// 시작 기준점은 바닥(y=0)으로 하여, 첫 섬부터 캐릭터가 도달할 수 있도록 합니다.
    /// </summary>
    private void GenerateIslands()
    {
        Debug.Log("공중 섬 생성 중...");

        float currentX = 0;
        // 시작 기준점을 바닥으로 설정 (y=0)
        float lastCenterY = 0;
        // 섬은 바닥 바로 위에서 생성되므로 최소 y값은 1로 설정
        float minIslandY = 1;

        while (currentX < mapSize.x)
        {
            // 섬의 반경 결정
            float radius = Random.Range(minIslandRadius, maxIslandRadius);

            // 섬의 중심 좌표: x는 currentX + radius,
            // y는 이전 섬 중심에서 [-playerJumpHeight, playerJumpHeight] 범위 내에서 결정하며, 최소 1 이상의 높이로 클램프
            float centerX = currentX + radius;
            float deltaY = Random.Range(-playerJumpHeight, playerJumpHeight);
            float centerY = Mathf.Clamp(lastCenterY + deltaY, minIslandY, mapSize.y - 1);
            Vector2 center = new Vector2(centerX, centerY);

            // 섬의 영역 계산
            int minX = Mathf.FloorToInt(centerX - radius);
            int maxX = Mathf.CeilToInt(centerX + radius);
            int minY = Mathf.FloorToInt(centerY - radius);
            int maxY = Mathf.CeilToInt(centerY + radius);

            // 영역 내 각 좌표에 대해 섬 타일 생성 여부 결정 (원 형태 내부 + Perlin 노이즈로 자연스러운 경계)
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    // 맵 범위 체크
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

            // 선택 사항: 섬 중앙에 아이템 및 몬스터 생성
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

            // 다음 섬까지의 수평 간격은 현재 섬의 폭(2 * radius)와 playerJumpDistance를 반영합니다.
            float jumpGap = playerJumpDistance * Random.Range(0.8f, 1.3f);
            currentX = centerX + radius + jumpGap;
            // 다음 섬의 수직 기준을 갱신
            lastCenterY = centerY;
        }

        Debug.Log("공중 섬 생성 완료!");
    }
}
