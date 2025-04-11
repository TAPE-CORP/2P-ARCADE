using UnityEngine;

public class OrderJudge : MonoBehaviour
{
    [Header("�ֹ� ���� ���")]
    public Transform checkButtonTransform; // ��ư ��ġ
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
                money.AddMoney(100); // ���� ����
                Destroy(order.gameObject); // �ֹ� �Ϸ�
                Destroy(item.gameObject); // ������ ����
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
