using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(Conveyor))]
public class ConveyorBeltScroller : MonoBehaviour
{
    [Header("스크롤 속도 (UV/sec)")]
    public float scrollSpeed = 0.5f;

    // Built‑in Unlit/Texture 셰이더의 메인 텍스쳐 프로퍼티
    const string TEX_PROP = "_MainTex";

    private Conveyor _conveyor;
    private float _baseZRotation;
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _mpb;

    void Awake()
    {
        _conveyor = GetComponent<Conveyor>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _mpb = new MaterialPropertyBlock();
        _baseZRotation = transform.eulerAngles.z;
    }

    void Update()
    {
        // 1) 벨트 방향에 따라 회전
        float targetZ = _conveyor.isRight
            ? _baseZRotation
            : _baseZRotation + 180f;
        transform.rotation = Quaternion.Euler(0f, 0f, targetZ);

        // 2) UV 스크롤
        _meshRenderer.GetPropertyBlock(_mpb);

        // 에디터에서 지정한 타일링 값을 읽어옵니다.
        Vector2 tiling = _meshRenderer.sharedMaterial.GetTextureScale(TEX_PROP);

        float dir = _conveyor.isRight ? 1f : -1f;
        float offset = Time.time * scrollSpeed * dir;

        // ST 벡터 = (타일링.x, 타일링.y, 오프셋, 0)
        _mpb.SetVector(
            TEX_PROP + "_ST",
            new Vector4(tiling.x, tiling.y, offset, 0f)
        );

        _meshRenderer.SetPropertyBlock(_mpb);
    }
}
