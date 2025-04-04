using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject tilePrefab;
    public float parallaxFactor = 0.5f;
    public float zOffset = 10f;

    public float spacingX = 1f;
    public float spacingY = 1f;

    private int tileCountX;
    private int tileCountY;
    private float tileWidth;
    private float tileHeight;

    private List<GameObject> tiles = new List<GameObject>();
    private Vector2Int currentTileOffset = Vector2Int.zero;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        InitTiles();
    }

    void Update()
    {
        UpdateTiles();
    }

    void InitTiles()
    {
        SpriteRenderer sr = tilePrefab.GetComponent<SpriteRenderer>();
        tileWidth = sr.bounds.size.x * spacingX;
        tileHeight = sr.bounds.size.y * spacingY;

        float camWidth = GetCameraWidth();
        float camHeight = GetCameraHeight();

        // 패럴럭스 밀림 여유 공간 고려
        float parallaxShiftX = Mathf.Abs(parallaxFactor - 1f) * camWidth;
        float parallaxShiftY = Mathf.Abs(parallaxFactor - 1f) * camHeight;

        tileCountX = Mathf.CeilToInt((camWidth + parallaxShiftX) / tileWidth) + 13;
        tileCountY = Mathf.CeilToInt((camHeight + parallaxShiftY) / tileHeight) + 8;

        for (int y = 0; y < tileCountY; y++)
        {
            for (int x = 0; x < tileCountX; x++)
            {
                GameObject tile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity, transform);
                tiles.Add(tile);
            }
        }
    }

    void UpdateTiles()
    {
        Vector3 camPos = mainCamera.transform.position;

        int index = 0;
        for (int y = 0; y < tileCountY; y++)
        {
            for (int x = 0; x < tileCountX; x++)
            {
                if (index >= tiles.Count) break;

                int gridX = x - tileCountX / 2;
                int gridY = y - tileCountY / 2;

                float baseX = (Mathf.Floor(camPos.x / tileWidth) + gridX) * tileWidth;
                float baseY = (Mathf.Floor(camPos.y / tileHeight) + gridY) * tileHeight;

                float offsetX = baseX + (camPos.x * (parallaxFactor - 1f));
                float offsetY = baseY + (camPos.y * (parallaxFactor - 1f));

                GameObject tile = tiles[index];
                tile.transform.position = new Vector3(offsetX, offsetY, zOffset);

                // 공기 원근 효과 적용
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    float distanceFactor = 1f - Mathf.Clamp01(parallaxFactor); // 0.0 = 가장 가까움, 1.0 = 가장 멀리
                    Color baseColor = Color.white;
                    baseColor.a = Mathf.Lerp(1f, 0.5f, distanceFactor); // 멀수록 투명
                    baseColor.r = Mathf.Lerp(1f, 0.8f, distanceFactor); // 멀수록 붉은기 줄어듦
                    baseColor.g = Mathf.Lerp(1f, 0.9f, distanceFactor); // 채도 감소
                    baseColor.b = Mathf.Lerp(1f, 1f, distanceFactor);   // 블루는 유지

                    sr.color = baseColor;
                }

                index++;
            }
        }
    }

    float GetCameraWidth() => mainCamera.orthographicSize * 2f * mainCamera.aspect;
    float GetCameraHeight() => mainCamera.orthographicSize * 2f;
}
