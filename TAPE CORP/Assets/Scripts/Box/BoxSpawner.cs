using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class BoxSpawner : MonoBehaviour
{
    [Header("�ڽ� ������")]
    public GameObject boxPrefab;
    public Tilemap groundTilemap;

    [Header("����ڽ� ũ�� ǥ�� TMP �ؽ�Ʈ")]
    public TMP_Text correctBoxText;

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
        int correctIndex = Random.Range(0, boxCount);

        for (int i = 0; i < boxCount; i++)
        {
            float xPos = Random.Range(xStart, xEnd);
            Vector3 spawnPos = new Vector3(xPos, ySpawn, 0f);

            GameObject box = Instantiate(boxPrefab, spawnPos, Quaternion.identity);

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

            //  �ڽ� ũ�� ���� (localScale�� 1�� �����ϰ�, size�� ����)
            box.transform.localScale = Vector3.one;

            BoxCollider2D col = box.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.size = size;
            }

            Box boxScript = box.GetComponent<Box>();
            if (boxScript != null)
                boxScript.groundTilemap = groundTilemap;

            //  ����ڽ� �ؽ�Ʈ ǥ��
            if (i == correctIndex)
            {
                BoxSizeDisplay sizeDisplay = box.GetComponent<BoxSizeDisplay>();
                if (sizeDisplay != null && correctBoxText != null)
                {
                    sizeDisplay.SetSizeTextTarget(correctBoxText);
                }
            }
        }
    }
}
