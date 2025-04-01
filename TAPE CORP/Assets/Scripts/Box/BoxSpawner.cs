using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class BoxSpawner : MonoBehaviour
{
    [Header("�ڽ� ������")]
    public GameObject boxPrefab;
    public Tilemap groundTilemap;

    [Header("���� ����")]
    public float xStart = 0f;
    public float xEnd = 10f;
    public float ySpawn = 20f;

    [Header("���� ����")]
    public int boxCount = 5;
    public Vector2 correctBoxSize = new Vector2(1.5f, 2.5f);
    public Vector2 minBoxSize = new Vector2(0.8f, 1.0f);
    public Vector2 maxBoxSize = new Vector2(2.0f, 3.0f);

    private void Start()
    {
        SpawnAllBoxes();
    }

    private void SpawnAllBoxes()
    {
        // ����ڽ� �ε����� �����ϰ� ����
        int correctIndex = Random.Range(0, boxCount);

        for (int i = 0; i < boxCount; i++)
        {
            float xPos = Random.Range(xStart, xEnd);
            Vector3 spawnPos = new Vector3(xPos, ySpawn, 0f);

            GameObject box = Instantiate(boxPrefab, spawnPos, Quaternion.identity);

            // ������ ����
            Vector2 size;
            if (i == correctIndex)
            {
                size = correctBoxSize;
                box.name = "box����ڽ�";
            }
            else
            {
                float width = Random.Range(minBoxSize.x, maxBoxSize.x);
                float height = Random.Range(minBoxSize.y, maxBoxSize.y);
                size = new Vector2(width, height);
            }

            box.transform.localScale = new Vector3(size.x, size.y, 1f);

            // groundTilemap ���� (����)
            Box boxScript = box.GetComponent<Box>();
            if (boxScript != null)
                boxScript.groundTilemap = groundTilemap;
        }
    }
}
