using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ResourceTri : MonoBehaviour
{
    [Header("������ ����")]
    [Tooltip("�ּ� ���� ��")]
    public float minIncrease = 2f;
    [Tooltip("�ִ� ���� ��")]
    public float maxIncrease = 5f;

    void Awake()
    {
        // Collider2D�� Ʈ���ŷ� ����
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ResourceCon.instance != null)
        {
            float amount = Random.Range(minIncrease, maxIncrease);
            ResourceCon.instance.Gage += amount;
            Debug.Log($"ResourceTri: Gage +{amount:F1} �� {ResourceCon.instance.Gage:F1}");
            Destroy(gameObject); // �ڿ� ȹ�� �� ������Ʈ ����
        }
    }
}
