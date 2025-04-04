using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerGenManager : MonoBehaviour
{
    public Camera mainCamera;
    public int spawnCount = 5;
    public float checkInterval = 1f;

    [Header("���� �Ÿ� ���� (ī�޶� ����)")]
    public float minSpawnDistanceFromCamera = 5f;  // �ּ� �Ÿ�
    public float maxSpawnDistanceFromCamera = 15f; // �ִ� �Ÿ�

    private List<GameObject> activeAObjects = new List<GameObject>();

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            CheckAndRespawn();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void CheckAndRespawn()
    {
        // ��Ȱ��ȭ�Ǿ��ų� null�� ������Ʈ ����
        for (int i = activeAObjects.Count - 1; i >= 0; i--)
        {
            if (activeAObjects[i] == null || !activeAObjects[i].activeInHierarchy)
            {
                activeAObjects.RemoveAt(i);
            }
        }

        while (activeAObjects.Count < spawnCount)
        {
            GameObject newWorker = SpawnAboveBridge();
            if (newWorker != null)
                activeAObjects.Add(newWorker);
            else
                break; // ���� ��ġ ����
        }
    }

    GameObject SpawnAboveBridge()
    {
        GameObject[] bridges = GameObject.FindGameObjectsWithTag("Bridge");
        List<GameObject> candidateBridges = new List<GameObject>();

        Vector3 camPos = mainCamera.transform.position;

        foreach (var bridge in bridges)
        {
            float dx = Mathf.Abs(bridge.transform.position.x - camPos.x);
            if (dx >= minSpawnDistanceFromCamera && dx <= maxSpawnDistanceFromCamera)
            {
                candidateBridges.Add(bridge);
            }
        }

        if (candidateBridges.Count == 0)
            return null;

        GameObject selectedBridge = candidateBridges[Random.Range(0, candidateBridges.Count)];

        SpriteRenderer bridgeSprite = selectedBridge.GetComponent<SpriteRenderer>();
        float topY = bridgeSprite != null ? bridgeSprite.bounds.max.y : selectedBridge.transform.position.y;
        float spawnOffsetY = 0.5f;

        Vector3 spawnPos = new Vector3(
            selectedBridge.transform.position.x,
            topY + spawnOffsetY,
            selectedBridge.transform.position.z
        );

        GameObject worker = WorkerPoolManager.Instance.GetWorker(spawnPos);
        return worker;
    }
}
