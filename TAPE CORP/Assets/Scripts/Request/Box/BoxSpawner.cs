using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public List<BoxPrefabSet> boxSets;
    public float spawnInterval = 2f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnBox();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBox()
    {
        int areaSize = Random.Range(1, 6); // 1~5

        BoxPrefabSet set = boxSets.Find(x => x.areaSize == areaSize);
        if (set == null)
        {
            Debug.LogWarning($"AreaSize {areaSize}에 해당하는 프리팹이 없습니다.");
            return;
        }

        GameObject newBox = Instantiate(set.unboxedPrefab, transform.position, Quaternion.identity);

        float sideLength = Mathf.Sqrt(areaSize);
        newBox.transform.localScale = new Vector3(sideLength, sideLength, 1f);

        BoxObject boxObj = newBox.GetComponent<BoxObject>();
        if (boxObj == null)
            boxObj = newBox.AddComponent<BoxObject>();

        boxObj.areaSize = areaSize;
        boxObj.packedPrefab = set.packedPrefab;
    }
}
