using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ShakeObj : MonoBehaviour
{
    // �ܺο��� �Ҵ��� weight (��: kg)
    [SerializeField] private float weight;

    [Header("���� ����ȭ ���ذ�")]
    [Tooltip("weight / maxWeight �� intensity �� ����մϴ�.")]
    [SerializeField] private float maxWeight = 10f;

    /// <summary>
    /// �ܺο��� ��ü�� �����ϸ鼭 ���� �Ǵ� ���ϴ� ������ ���Ը� �Ҵ��� �� ȣ���ϼ���.
    /// </summary>
    public void SetWeight(float w)
    {
        weight = w;
    }

    /// <summary>
    /// �÷��̾ �� ������Ʈ�� ���ø� ��, Ȥ�� �浹 �� ȣ��˴ϴ�.
    /// </summary>
    public void OnLifted()
    {
        if (SoundIO.Instance == null)
        {
            Debug.LogWarning("SoundIO �ν��Ͻ��� ã�� �� �����ϴ�.");
            return;
        }

        // ������ weight / maxWeight ������ intensity ��� (0~1)
        float intensity = Mathf.Clamp01(weight / maxWeight);

        // ���� ���
        SoundIO.Instance.PlayWithIntensity(intensity);
    }

    /// <summary>
    /// Map ���̾��� ������Ʈ�� �浹���� ���� OnLifted�� ȣ���մϴ�.
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        // "Map" ���̾� �̸��� �̿��� ���̾� �ε����� ����
        int mapLayer = LayerMask.NameToLayer("Map");
        if (collision.gameObject.layer == mapLayer)
        {
            OnLifted();
        }
    }
}
