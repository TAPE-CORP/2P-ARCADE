using UnityEngine;
using TMPro;

public class BoxSizeDisplay : MonoBehaviour
{
    private BoxCollider2D col;
    private TMP_Text sizeText;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    public void SetSizeTextTarget(TMP_Text target)
    {
        sizeText = target;
        UpdateSizeText();
    }

    public void UpdateSizeText()
    {
        if (col != null && sizeText != null)
        {
            // ���� ���� ũ�� �ݿ�
            Vector2 size = new Vector2(
                col.size.x * transform.lossyScale.x,
                col.size.y * transform.lossyScale.y
            );

            sizeText.text = $"{size.x:F1} * {size.y:F1}";
        }
    }
}
