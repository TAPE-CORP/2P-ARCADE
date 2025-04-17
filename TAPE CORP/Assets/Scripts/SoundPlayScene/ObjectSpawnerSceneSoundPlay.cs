using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnerSceneSoundPlay : MonoBehaviour
{
    [SerializeField] private ShakeObj shakePrefab;
    [SerializeField] private float minRandomWeight = 1f;
    [SerializeField] private float maxRandomWeight = 10f;
    [SerializeField] private float spawnInterval = 5f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnRandomShakeObj), 0f, spawnInterval);//���߿� Ư�� ���� �̸� ȣ��ǵ��� �ٲ����
    }

    private void SpawnRandomShakeObj()
    {
        // ���� ��ġ �� ȸ�� ���� (���ϴ� ��� ����)
        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = Quaternion.identity;

        // ������ �ν��Ͻ�ȭ
        var instance = Instantiate(shakePrefab, spawnPos, spawnRot);

        // ���� ���� �Ҵ�
        float randomWeight = Random.Range(minRandomWeight, maxRandomWeight);
        instance.SetWeight(randomWeight);

        // (��: ���� ���� �Ͽ�) ��ü�� ���÷��� �� ȣ��
        // instance.OnLifted();
    }
}
