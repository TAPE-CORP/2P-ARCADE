using UnityEngine;
using TMPro;

public class BoxSpawner : MonoBehaviour
{
    [Header("박스 프리팹")]
    public GameObject boxPrefab;

    [Header("정답박스 크기 표시 TMP 텍스트")]
    public TMP_Text correctBoxText;

    [Header("스폰 범위")]
    public float xStart = 0f;
    public float xEnd = 10f;
    public float ySpawn = 20f;

    [Header("생성 설정")]
    public int boxCount = 5;
    public Vector2 minBoxScale = new Vector2(0.8f, 1.0f);
    public Vector2 maxBoxScale = new Vector2(2.0f, 3.0f);

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

            // ✅ 스프라이트 크기만 조절
            float scaleX = Random.Range(minBoxScale.x, maxBoxScale.x);
            float scaleY = Random.Range(minBoxScale.y, maxBoxScale.y);
            box.transform.localScale = new Vector3(scaleX, scaleY, 1f);

            // ✅ 정답박스 처리
            if (i == correctIndex)
            {
                box.name = "box정답박스";

                BoxSizeDisplay sizeDisplay = box.GetComponent<BoxSizeDisplay>();
                if (sizeDisplay != null && correctBoxText != null)
                {
                    sizeDisplay.SetSizeTextTarget(correctBoxText);
                }
            }
        }
    }
}
