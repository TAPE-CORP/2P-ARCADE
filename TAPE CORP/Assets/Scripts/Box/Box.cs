using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Box : MonoBehaviour
{
    public Tilemap groundTilemap; // Assign externally

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (IsTouchingGround())
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private bool IsTouchingGround()
    {
        Vector3Int cellBelow = groundTilemap.WorldToCell(transform.position + Vector3.down * 0.5f);
        return groundTilemap.HasTile(cellBelow);
    }
}
