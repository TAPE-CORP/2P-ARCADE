using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public static ObjectSpawner Instance;

    public SpawnableData[] spawnableDatas;
    public Transform spawnPoint;
    public float spawnInterval = 2f;

    public List<SpawnedObject> activeObjects = new List<SpawnedObject>();
    private float timer;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomObject();
        }
    }

    void SpawnRandomObject()
    {
        if (spawnableDatas.Length == 0 || spawnPoint == null)
            return;

        int index = Random.Range(0, spawnableDatas.Length);
        SpawnableData data = spawnableDatas[index];

        GameObject obj = Instantiate(data.prefab, spawnPoint.position, Quaternion.identity);
        SpawnedObject spawned = obj.AddComponent<SpawnedObject>();
        spawned.data = data;


        activeObjects.Add(spawned);
    }

    public void RemoveObject(SpawnedObject obj)
    {
        if (activeObjects.Contains(obj))
        {
            activeObjects.Remove(obj);
        }
    }
}
