using System.Collections.Generic;
using UnityEngine;

public class WorkerPoolManager : MonoBehaviour
{
    public static WorkerPoolManager Instance;

    public GameObject workerPrefab;
    public int initialPoolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(workerPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetWorker(Vector3 position)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(workerPrefab);
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnWorker(GameObject worker)
    {
        worker.SetActive(false);
        pool.Enqueue(worker);
    }
}
