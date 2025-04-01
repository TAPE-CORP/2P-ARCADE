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

        tileCountX = Mathf.CeilToInt((camWidth + parallaxShiftX) / tileWidth) + 50;
        tileCountY = Mathf.CeilToInt((camHeight + parallaxShiftY) / tileHeight) + 50;

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

                // 카메라 기준 타일 좌표
                float baseX = (Mathf.Floor(camPos.x / tileWidth) + gridX) * tileWidth;
                float baseY = (Mathf.Floor(camPos.y / tileHeight) + gridY) * tileHeight;

                // 패럴럭스로 밀릴 거리 계산 → 반대로 보정
                float offsetX = baseX + (camPos.x * (parallaxFactor - 1f));
                float offsetY = baseY + (camPos.y * (parallaxFactor - 1f));

                tiles[index].transform.position = new Vector3(offsetX, offsetY, zOffset);
                index++;
            }
        }
    }

    float GetCameraWidth() => mainCamera.orthographicSize * 2f * mainCamera.aspect;
    float GetCameraHeight() => mainCamera.orthographicSize * 2f;
}
