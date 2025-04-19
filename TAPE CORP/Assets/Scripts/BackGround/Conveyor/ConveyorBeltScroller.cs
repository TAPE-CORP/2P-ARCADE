// ConveyorBeltScroller.cs
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(Conveyor))]
public class ConveyorBeltScroller : MonoBehaviour
{
    [Header("스크롤 속도 (UV/sec)")]
    public float scrollSpeed = 0.5f;

    const string TEX_PROP = "_MainTex";

    private Conveyor _conveyor;
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _mpb;

    void Awake()
    {
        _conveyor = GetComponent<Conveyor>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        // 1) 회전은 Conveyor가 담당하므로 여기선 UV만
        _meshRenderer.GetPropertyBlock(_mpb);
        Vector2 tiling = _meshRenderer.sharedMaterial.GetTextureScale(TEX_PROP);

        // transform.right.x 부호로 스크롤 방향 결정
        float dir = transform.right.x >= 0f ? 1f : -1f;
        float offset = Time.time * scrollSpeed * dir;

        _mpb.SetVector(TEX_PROP + "_ST", new Vector4(
            tiling.x, tiling.y,
            offset, 0f
        ));
        _meshRenderer.SetPropertyBlock(_mpb);
    }
}
