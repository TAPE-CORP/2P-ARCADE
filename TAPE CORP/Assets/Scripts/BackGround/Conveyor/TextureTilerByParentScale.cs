using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TextureTilerByParentScale : MonoBehaviour
{
    public Vector2 tilingMultiplier = Vector2.one;

    private Renderer rend;
    private Vector3 lastParentScale;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (transform.parent != null)
        {
            lastParentScale = transform.parent.localScale;
            UpdateTiling();
        }
    }

    void UpdateTiling()
    {
        Vector3 parentScale = transform.parent.localScale;
        Vector2 newTiling = new Vector2(parentScale.x * tilingMultiplier.x, parentScale.y * tilingMultiplier.y);
        rend.material.mainTextureScale = newTiling;
    }
}
