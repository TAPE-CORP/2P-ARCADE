using UnityEngine;

public class DoorFrameSpawner : MonoBehaviour
{
    [Header("��Ʋ ������")]
    public GameObject doorFramePrefab;

    [Header("���� ��ġ ����")]
    public float xStart = -5f;
    public float xEnd = 5f;
    public float ySpawn = 10f;

    [Header("�ٴ� ���̾�")]
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

        // ������ ��ũ��Ʈ ���̱�
        if (spawnedFrame.GetComponent<DoorFrame>() == null)
        {
            spawnedFrame.AddComponent<DoorFrame>();
        }

        spawnedFrame.GetComponent<DoorFrame>().groundMask = groundMask;
    }
}
