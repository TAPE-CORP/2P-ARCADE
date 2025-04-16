using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    public MultiTargetCamera cameraController; // ī�޶� ��ũ��Ʈ ����

    public LayerMask mapLayerMask;
    public float checkRadius = 0.5f;
    public int maxAttempts = 30;
    public Vector2 spawnAreaMin = new Vector2(-10, 0);
    public Vector2 spawnAreaMax = new Vector2(10, 10);

    void Start()
    {
        Vector2 player1Pos = GetSafeSpawnPosition();
        Vector2 player2Pos = GetSafeSpawnPosition(avoidPos: player1Pos);

        GameObject p1 = Instantiate(player1Prefab, player1Pos, Quaternion.identity, this.transform);
        GameObject p2 = Instantiate(player2Prefab, player2Pos, Quaternion.identity, this.transform);

        // ī�޶� Ÿ�� ����Ʈ�� ���
        if (cameraController != null)
        {
            cameraController.targets = new List<Transform> { p1.transform, p2.transform };
        }
        else
        {
            Debug.LogWarning("ī�޶� ��Ʈ�ѷ��� ������� �ʾҽ��ϴ�!");
        }
    }

    Vector2 GetSafeSpawnPosition(Vector2? avoidPos = null, float avoidMinDistance = 1.5f)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            bool isOverlapping = Physics2D.OverlapCircle(randomPos, checkRadius, mapLayerMask);
            bool isTooClose = avoidPos.HasValue && Vector2.Distance(randomPos, avoidPos.Value) < avoidMinDistance;

            if (!isOverlapping && !isTooClose)
                return randomPos;
        }

        Debug.LogWarning("������ ��ġ�� ã�� ����");
        return Vector2.zero;
    }
}
