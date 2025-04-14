using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectAPrefab;
    public GameObject objectDPrefab;
    public float spawnInterval = 2f;
    [Range(0f, 1f)] public float dSpawnChance = 0.3f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnObjectA), 0f, spawnInterval);
    }

    void SpawnObjectA()
    {
        Vector3 spawnPosition = new Vector3(-2f, 50f, 0f);
        GameObject aInstance = Instantiate(objectAPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = aInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // A의 Rigidbody2D를 1초 후 Kinematic으로 변경
        StartCoroutine(SetKinematicAfterDelay(aInstance));

        // D 생성 조건
        if (Random.value < dSpawnChance)
        {
            Transform dSpawnPoint = aInstance.transform.Find("DSpawnPoint");

            if (dSpawnPoint != null)
            {
                GameObject dInstance = Instantiate(objectDPrefab, dSpawnPoint.position, Quaternion.identity, aInstance.transform);
                Rigidbody2D dRb = dInstance.GetComponent<Rigidbody2D>();
                if (dRb != null)
                {
                    dRb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
            else
            {
                Debug.LogWarning("DSpawnPoint not found in A prefab");
            }
        }
    }

    IEnumerator SetKinematicAfterDelay(GameObject aInstance)
    {
        yield return new WaitForSeconds(1f);

        Rigidbody2D rb = aInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
