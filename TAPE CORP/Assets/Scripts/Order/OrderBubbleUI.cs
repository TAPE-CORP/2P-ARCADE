using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderBubbleUI : MonoBehaviour
{
    [Header("UI 요소")]
    public TMP_Text itemNameText;
    public Image boxImage;
    public TMP_Text boxSizeText;
    public TMP_Text itemSizeText;
    public Image bubbleBackground;

    [Header("타이머 색상")]
    public Color fullTimeColor;
    public Color midTimeColor;
    public Color lowTimeColor;

    private float remainingTime;
    private float maxTime;

    public OrderData data;

    public void Init(OrderData order)
    {
        data = order;
        maxTime = remainingTime = order.timeLimit;

        itemNameText.text = order.itemName;
        boxImage.sprite = order.boxImage;
        boxSizeText.text = $"Box: {order.boxSize.x} x {order.boxSize.y}";
        itemSizeText.text = $"Item: {order.itemSize.x} x {order.itemSize.y}";

        UpdateColor();
    }

    private void Update()
    {
        if (remainingTime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        remainingTime -= Time.deltaTime;
        UpdateColor();
    }

    private void UpdateColor()
    {
        float ratio = remainingTime / maxTime;
        if (ratio > 0.66f)
            bubbleBackground.color = fullTimeColor;
        else if (ratio > 0.33f)
            bubbleBackground.color = midTimeColor;
        else
            bubbleBackground.color = lowTimeColor;
    }
}
