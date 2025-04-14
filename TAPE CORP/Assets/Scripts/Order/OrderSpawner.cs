using UnityEngine;
using System.Collections.Generic;

public class OrderSpawner : MonoBehaviour
{
    [Header("�ֹ� UI ������ �� �θ�")]
    public GameObject orderBubblePrefab;
    public Transform bubbleParent;

    [Header("���� ������ �ֹ� ����Ʈ")]
    public List<OrderData> possibleOrders;

    public void SpawnRandomOrder()
    {
        if (possibleOrders.Count == 0) return;

        int index = Random.Range(0, possibleOrders.Count);
        OrderData selected = possibleOrders[index];

        GameObject go = Instantiate(orderBubblePrefab, bubbleParent);
        OrderBubbleUI bubble = go.GetComponent<OrderBubbleUI>();

        // UI ������ ���� ���� UI �б�
        foreach (Transform child in bubbleParent)
        {
            if (child != go.transform)
                child.localPosition += new Vector3(200, 0, 0); // ���������� �б�
        }

        // ���̵��� ���� �ð� ���� (1~2��)
        float sizeFactor = (selected.boxSize.x + selected.boxSize.y + selected.itemSize.x + selected.itemSize.y) / 4f;
        selected.timeLimit = Mathf.Clamp(60f + sizeFactor * 10f, 60f, 120f);

        bubble.Init(selected);
    }
}
