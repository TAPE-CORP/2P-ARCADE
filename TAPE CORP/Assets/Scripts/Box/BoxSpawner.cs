<<<<<<< HEAD
using UnityEngine;
using UnityEngine.Tilemaps;
=======
﻿using UnityEngine;
>>>>>>> main
using TMPro;

public class BoxSpawner : MonoBehaviour
{
    [Header("박스 프리팹")]
    public GameObject boxPrefab;

<<<<<<< HEAD
    [Header("����ڽ� ũ�� ǥ�� TMP �ؽ�Ʈ")]
    public TMP_Text correctBoxText;

    [Header("���� ����")]
=======
    [Header("정답박스 크기 표시 TMP 텍스트")]
    public TMP_Text correctBoxText;

    [Header("스폰 범위")]
>>>>>>> main
    public float xStart = 0f;
    public float xEnd = 10f;
    public float ySpawn = 20f;

    [Header("생성 설정")]
    public int boxCount = 5;
    public Vector2 minBoxScale = new Vector2(0.8f, 1.0f);
    public Vector2 maxBoxScale = new Vector2(2.0f, 3.0f);

    [Header("디버그")]
    [SerializeField] private Vector2 correctBoxActualSize;

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

<<<<<<< HEAD
            Vector2 size;
=======
            // ✅ 랜덤 스케일 설정
            float scaleX = Random.Range(minBoxScale.x, maxBoxScale.x);
            float scaleY = Random.Range(minBoxScale.y, maxBoxScale.y);
            Vector3 scale = new Vector3(scaleX, scaleY, 1f);
            box.transform.localScale = scale;

>>>>>>> main
            if (i == correctIndex)
            {
                box.name = "box정답박스";

                SpriteRenderer sr = box.GetComponent<SpriteRenderer>();
                if (sr != null && correctBoxText != null)
                {
                    Vector2 worldSize = sr.bounds.size;
                    correctBoxActualSize = worldSize;
                    correctBoxText.text = $"{worldSize.x:F1} * {worldSize.y:F1}";
                }
            }

<<<<<<< HEAD
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
=======
>>>>>>> main
        }
    }
}
