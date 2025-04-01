<<<<<<< HEAD
using UnityEngine;
using TMPro;

=======
ï»¿using UnityEngine;
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
        UpdateSizeText(); // âœ… í…ìŠ¤íŠ¸ ê°±ì‹ ì€ ì—¬ê¸°ì„œ ì¦‰ì‹œ ì‹¤í–‰
>>>>>>> main
    }

    public void UpdateSizeText()
    {
        if (col != null && sizeText != null)
        {
<<<<<<< HEAD
            // ½ÇÁ¦ ¿ùµå Å©±â ¹Ý¿µ
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
            Debug.LogWarning("BoxSizeDisplay: sizeText ë˜ëŠ” colì´ nullìž…ë‹ˆë‹¤.");
        }
>>>>>>> main
    }
}
