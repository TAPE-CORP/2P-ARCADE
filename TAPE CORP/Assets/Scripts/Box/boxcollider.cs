using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class ColliderAutoResizer : MonoBehaviour
{
    private SpriteRenderer sr;
    private BoxCollider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    void LateUpdate()
    {
        // ❗ pivot 기준 로컬 사이즈 가져오기
        Vector2 size = sr.sprite.bounds.size;
        col.size = size;

        // 하단 기준 offset 적용
        col.offset = new Vector2(0f, size.y * 0f);
    }
}
