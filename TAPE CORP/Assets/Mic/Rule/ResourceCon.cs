using UnityEngine;

public class ResourceCon : MonoBehaviour
{
    public static ResourceCon instance;

    [Header("Resource Settings")]
    public float Gage = 100f;

    [Header("Minus Speed ����")]
    [Tooltip("������ ���� ���� �⺻ ���� �ӵ�")]
    public float defaultMinusSpd = 1f;
    [Tooltip("�����̴� �÷��̾� 1�δ� ������ ���")]
    public float upSpd = 1.8f;

    public float minusSpd;

    void Awake()
    {
        instance = this;
        minusSpd = defaultMinusSpd;
    }

    void Update()
    {
        // �� ������ ������ ���� ��, �����̴� Player ������ ���� minusSpd ����
        RecalculateMinusSpd();

        Gage -= minusSpd * Time.deltaTime;
        Debug.Log($"���� �ڿ� = {Gage:f1}, minusSpd = {minusSpd:f2}");
    }

    private void RecalculateMinusSpd()
    {
        // �±װ� "Player"�� ��� ������Ʈ�� ã�´�
        var players = GameObject.FindGameObjectsWithTag("Player");
        int movingCount = 0;

        foreach (var p in players)
        {
            var rb = p.GetComponent<Rigidbody2D>();
            if (rb != null && rb.velocity.magnitude > 0.1f)
                movingCount++;
        }

        // defaultMinusSpd �� upSpd^movingCount
        minusSpd = defaultMinusSpd * Mathf.Pow(upSpd, movingCount);
    }
}
