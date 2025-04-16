using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Reflection;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(ShadowCaster2D))]
public class SyncShadowWithCollider : MonoBehaviour
{
    void Awake()
    {
        var poly = GetComponent<PolygonCollider2D>();
        var shadow = GetComponent<ShadowCaster2D>();

        if (poly.pathCount != 1)
        {
            Debug.LogWarning("다중 path는 현재 미지원.");
            return;
        }

        Vector2[] colliderPath = poly.GetPath(0);
        Vector3[] shape = new Vector3[colliderPath.Length];
        for (int i = 0; i < colliderPath.Length; i++)
            shape[i] = colliderPath[i];

        shadow.useRendererSilhouette = false;
        shadow.selfShadows = true;

        // 리플렉션으로 SetShapePath 호출
        var method = typeof(ShadowCaster2D).GetMethod("SetShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(shadow, new object[] { shape });
        }
        else
        {
            Debug.LogError("SetShapePath 메서드를 찾을 수 없습니다.");
        }
    }
}
