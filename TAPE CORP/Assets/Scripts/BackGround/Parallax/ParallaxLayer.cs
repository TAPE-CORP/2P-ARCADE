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

    private float tileWidth;
    private float tileHeight;

    private List<GameObject> tiles = new List<GameObject>();
    private List<Vector2> tileBasePositions = new List<Vector2>();

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

        int countX = Mathf.FloorToInt(200f / tileWidth);
        int countY = Mathf.FloorToInt(100f / tileHeight);

        Vector2 center = new Vector2(100f, 50f);

        for (int y = 0; y <= countY; y++)
        {
            for (int x = 0; x <= countX; x++)
            {
                float baseX = (x - countX / 2f) * tileWidth + center.x;
                float baseY = (y - countY / 2f) * tileHeight + center.y;

                Vector2 basePos = new Vector2(baseX, baseY);
                tileBasePositions.Add(basePos);

                GameObject tile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity, transform);
                tiles.Add(tile);
            }
        }
    }

    void UpdateTiles()
    {
        Vector3 camPos = mainCamera.transform.position;

        for (int i = 0; i < tiles.Count; i++)
        {
            Vector2 basePos = tileBasePositions[i];

            float offsetX = basePos.x + camPos.x * (parallaxFactor - 1f);
            float offsetY = basePos.y + camPos.y * (parallaxFactor - 1f);

            GameObject tile = tiles[i];
            tile.transform.position = new Vector3(offsetX, offsetY, zOffset);

            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float distanceFactor = 1f - Mathf.Clamp01(parallaxFactor);
                Color baseColor = Color.white;
                baseColor.a = Mathf.Lerp(1f, 0.5f, distanceFactor);
                baseColor.r = Mathf.Lerp(1f, 0.8f, distanceFactor);
                baseColor.g = Mathf.Lerp(1f, 0.9f, distanceFactor);
                baseColor.b = Mathf.Lerp(1f, 1f, distanceFactor);
                sr.color = baseColor;
            }
        }
    }
}
