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
        InvokeRepeating(nameof(SpawnRandomShakeObj), 0f, spawnInterval);//나중에 특정 조건 이면 호출되도록 바꿔야함
    }

    private void SpawnRandomShakeObj()
    {
        // 스폰 위치 및 회전 예시 (원하는 대로 변경)
        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = Quaternion.identity;

        // 프리팹 인스턴스화
        var instance = Instantiate(shakePrefab, spawnPos, spawnRot);

        // 랜덤 무게 할당
        float randomWeight = Random.Range(minRandomWeight, maxRandomWeight);
        instance.SetWeight(randomWeight);

        // (예: 일정 조건 하에) 객체가 들어올려질 때 호출
        // instance.OnLifted();
    }
}
