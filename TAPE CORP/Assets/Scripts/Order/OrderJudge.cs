using UnityEngine;

public class OrderJudge : MonoBehaviour
{
    [Header("주문 검증 대상")]
    public Transform checkButtonTransform; // 버튼 위치
    public float checkRadius = 0.5f;
    public LayerMask itemLayer;

    public void CheckOrders()
    {
        Collider2D hit = Physics2D.OverlapCircle(checkButtonTransform.position, checkRadius, itemLayer);
        if (hit == null) return;

        OrderItem item = hit.GetComponent<OrderItem>();
        if (item == null) return;

        OrderBubbleUI[] orders = FindObjectsOfType<OrderBubbleUI>();
        foreach (var order in orders)
        {
            if (IsMatch(item, order.data))
            {
                MoneyManager money = FindObjectOfType<MoneyManager>();
                money.AddMoney(100); // 정답 보상
                Destroy(order.gameObject); // 주문 완료
                Destroy(item.gameObject); // 아이템 제거
                break;
            }
        }
    }

    private bool IsMatch(OrderItem item, OrderData data)
    {
        return item.itemName == data.itemName &&
               Mathf.Approximately(item.size.x, data.itemSize.x) &&
               Mathf.Approximately(item.size.y, data.itemSize.y);
    }
}
