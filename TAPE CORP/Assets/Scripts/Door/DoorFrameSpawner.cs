using UnityEngine;

public class DoorFrameSpawner : MonoBehaviour
{
    [Header("문틀 프리팹")]
    public GameObject doorFramePrefab;

    [Header("스폰 위치 설정")]
    public float xStart = -5f;
    public float xEnd = 5f;
    public float ySpawn = 10f;

    [Header("바닥 레이어")]
    public LayerMask groundMask;

    private GameObject spawnedFrame;

    private void Start()
    {
        SpawnDoorFrame();
    }

    void SpawnDoorFrame()
    {
        float xPos = Random.Range(xStart, xEnd);
        Vector3 spawnPos = new Vector3(xPos, ySpawn, 0f);
        spawnedFrame = Instantiate(doorFramePrefab, spawnPos, Quaternion.identity);

        // 감지용 스크립트 붙이기
        if (spawnedFrame.GetComponent<DoorFrame>() == null)
        {
            spawnedFrame.AddComponent<DoorFrame>();
        }

        spawnedFrame.GetComponent<DoorFrame>().groundMask = groundMask;
    }
}
