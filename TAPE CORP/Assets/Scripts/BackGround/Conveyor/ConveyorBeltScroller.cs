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
        baseZRotation = transform.eulerAngles.z; // ���� ���� ����
    }

    void Update()
    {
        // ���� ���� + ���⿡ ���� 0 �Ǵ� 180�� �߰�
        float targetZ = conveyor.isRight ? baseZRotation : baseZRotation + 180f;
        transform.rotation = Quaternion.Euler(0f, 0f, targetZ);

        // �ؽ�ó ��ũ��
        float direction = 1f;
        float offset = Time.time * scrollSpeed * direction;
        conveyorMaterial.mainTextureOffset = new Vector2(offset, 0);
    }
}
