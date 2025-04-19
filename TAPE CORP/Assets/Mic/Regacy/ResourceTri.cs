using UnityEngine;
using UnityEngine.Rendering.Universal;  // Light2D ����� ����

[RequireComponent(typeof(Collider2D))]
public class ResourceTri : MonoBehaviour
{
    [Header("������ ����")]
    [Tooltip("�ּ� ���� ��")]
    public float minIncrease = 2f;
    [Tooltip("�ִ� ���� ��")]
    public float maxIncrease = 5f;

    [Header("���� �÷��� ����")]
    [Tooltip("�÷��õ� �ִ� ���")]
    public float flashIntensity = 5f;
    [Tooltip("�÷��� ���� �ð� (��)")]
    public float flashDuration = 0.1f;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ResourceCon.instance != null)
        {
            // �ڿ� ����
            float amount = Random.Range(minIncrease, maxIncrease);
            ResourceCon.instance.Gage += amount;
            Debug.Log($"ResourceTri: Gage +{amount:F1} �� {ResourceCon.instance.Gage:F1}");

            // ResourceCon ������ �÷��� ��û
            ResourceCon.instance.RequestFlash(flashIntensity, flashDuration);

            // ȹ�� �� ����
            Destroy(gameObject);
        }
    }
}
