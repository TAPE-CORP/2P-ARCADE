using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerGenManager : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject prefabA;
    public int spawnCount = 5;
    public LayerMask bridgeLayer;
    public float checkInterval = 0.5f;

    private List<GameObject> activeAObjects = new List<GameObject>();

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1f); // 시작 1초 후 실행

        while (true)
        {
            CheckAndRespawn();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void CheckAndRespawn()
    {
        for (int i = activeAObjects.Count - 1; i >= 0; i--)
        {
            if (activeAObjects[i] == null)
            {
                activeAObjects.RemoveAt(i);
                continue;
            }

            WorkerMover mover = activeAObjects[i].GetComponent<WorkerMover>();

            // 카메라에 들어온 적 있고, 현재 카메라 밖이면 제거
            if (activeAObjects[i] == null)
            {
                activeAObjects.RemoveAt(i);
            }
        }

        while (activeAObjects.Count < spawnCount)
        {
            SpawnAFromSide();
        }
    }

    bool IsInCameraView(Vector3 position)
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(position);
        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
               viewportPos.y >= 0 && viewportPos.y <= 1 &&
               viewportPos.z >= 0;
    }

    void SpawnAFromSide()
    {
        float camWidth = mainCamera.orthographicSize * 2f * mainCamera.aspect;
        float camHeight = mainCamera.orthographicSize * 2f;

        float camLeft = mainCamera.transform.position.x - camWidth / 2f;
        float camRight = mainCamera.transform.position.x + camWidth / 2f;
        float camY = mainCamera.transform.position.y;

        float spawnY = camY + Random.Range(-camHeight / 4f, camHeight / 4f);

        bool fromLeft = Random.value < 0.5f;
        float spawnX = fromLeft ? camLeft - 2f : camRight + 2f;
        float targetX = fromLeft ? camRight + 2f : camLeft - 2f;

        Vector3 spawnPos = new Vector3(spawnX, spawnY + 0.1f, 0f);
        Vector3 targetPos = new Vector3(targetX, spawnY, 0f);

        GameObject a = Instantiate(prefabA, spawnPos, Quaternion.identity);
        activeAObjects.Add(a);

        WorkerMover mover = a.GetComponent<WorkerMover>();
        if (mover != null)
        {
            mover.MoveTo(targetPos);
        }
    }
}
