using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ConveyorBeltScroller : MonoBehaviour
{
    public Material conveyorMaterial;
    public float scrollSpeed = 0.5f;
    public Vector2 baseTiling = new Vector2(1f, 1f); // 단위 스케일당 반복 횟수
    public float baseTilingX = 5f; // X축 단위 스케일당 반복 횟수

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // 초기 스케일 기준 반복 횟수 설정
        Vector3 scale = transform.lossyScale;
        Vector2 tiling = new Vector2(scale.x * baseTiling.x/5, scale.y * baseTiling.y);
        conveyorMaterial.mainTextureScale = tiling;
    }

    void Update()
    {
        // 텍스처 스크롤만 계속
        float offset = Time.time * scrollSpeed;
        conveyorMaterial.mainTextureOffset = new Vector2(offset, 0);
    }
}
