using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ConveyorBeltScroller : MonoBehaviour
{
    public Material conveyorMaterial;
    public float scrollSpeed = 0.5f;

    private Conveyor conveyor;
    private float baseZRotation;

    void Start()
    {
        conveyor = GetComponent<Conveyor>();
        baseZRotation = transform.eulerAngles.z; // 시작 각도 저장
    }

    void Update()
    {
        // 기존 각도 + 방향에 따라 0 또는 180도 추가
        float targetZ = conveyor.isRight ? baseZRotation : baseZRotation + 180f;
        transform.rotation = Quaternion.Euler(0f, 0f, targetZ);

        // 텍스처 스크롤
        float direction = 1f;
        float offset = Time.time * scrollSpeed * direction;
        conveyorMaterial.mainTextureOffset = new Vector2(offset, 0);
    }
}
