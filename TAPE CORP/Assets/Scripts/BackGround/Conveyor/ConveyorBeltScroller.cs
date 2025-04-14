using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ConveyorBeltScroller : MonoBehaviour
{
    public Material conveyorMaterial;
    public float scrollSpeed = 0.5f;
    public Vector2 baseTiling = new Vector2(1f, 1f); // ���� �����ϴ� �ݺ� Ƚ��
    public float baseTilingX = 5f; // X�� ���� �����ϴ� �ݺ� Ƚ��

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // �ʱ� ������ ���� �ݺ� Ƚ�� ����
        Vector3 scale = transform.lossyScale;
        Vector2 tiling = new Vector2(scale.x * baseTiling.x/5, scale.y * baseTiling.y);
        conveyorMaterial.mainTextureScale = tiling;
    }

    void Update()
    {
        // �ؽ�ó ��ũ�Ѹ� ���
        float offset = Time.time * scrollSpeed;
        conveyorMaterial.mainTextureOffset = new Vector2(offset, 0);
    }
}
