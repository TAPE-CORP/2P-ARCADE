using UnityEngine;

public class AlwaysFacingText : MonoBehaviour
{
    private Transform parent;

    void Start()
    {
        parent = transform.parent;
    }

    void LateUpdate()
    {
        if (parent == null) return;

        Vector3 scale = transform.localScale;
        scale.x = parent.localScale.x < 0 ? -1f : 1f;
        transform.localScale = scale;
    }
}
