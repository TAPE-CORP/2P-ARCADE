using UnityEngine;
using System.Collections.Generic;

public class OrderSpawner : MonoBehaviour
{
    [Header("주문 UI 프리팹 및 부모")]
    public GameObject orderBubblePrefab;
    public Transform bubbleParent;

    [Header("생성 가능한 주문 리스트")]
    public List<OrderData> possibleOrders;

    public void SpawnRandomOrder()
    {
        if (possibleOrders.Count == 0) return;

        int index = Random.Range(0, possibleOrders.Count);
        OrderData selected = possibleOrders[index];

        GameObject go = Instantiate(orderBubblePrefab, bubbleParent);
        OrderBubbleUI bubble = go.GetComponent<OrderBubbleUI>();

        // UI 정렬을 위해 기존 UI 밀기
        foreach (Transform child in bubbleParent)
        {
            if (child != go.transform)
                child.localPosition += new Vector3(200, 0, 0); // 오른쪽으로 밀기
        }

        // 난이도에 따라 시간 설정 (1~2분)
        float sizeFactor = (selected.boxSize.x + selected.boxSize.y + selected.itemSize.x + selected.itemSize.y) / 4f;
        selected.timeLimit = Mathf.Clamp(60f + sizeFactor * 10f, 60f, 120f);

        bubble.Init(selected);
    }
}
