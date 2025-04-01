<<<<<<< HEAD
using UnityEngine;
using TMPro;

=======
﻿using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
>>>>>>> main
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
<<<<<<< HEAD
        UpdateSizeText();
=======
        UpdateSizeText(); // ✅ 텍스트 갱신은 여기서 즉시 실행
>>>>>>> main
    }

    public void UpdateSizeText()
    {
        if (col != null && sizeText != null)
        {
<<<<<<< HEAD
            // ���� ���� ũ�� �ݿ�
            Vector2 size = new Vector2(
                col.size.x * transform.lossyScale.x,
                col.size.y * transform.lossyScale.y
            );

            sizeText.text = $"{size.x:F1} * {size.y:F1}";
        }
=======
            Vector2 size = col.size;
            sizeText.text = $"{size.x:F1} * {size.y:F1}";
        }
        else
        {
            Debug.LogWarning("BoxSizeDisplay: sizeText 또는 col이 null입니다.");
        }
>>>>>>> main
    }
}
