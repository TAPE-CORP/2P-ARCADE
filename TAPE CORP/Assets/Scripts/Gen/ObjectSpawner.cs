using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab;
    public Transform SpawnTransform;
    public float spawnInterval = 2f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnObjectD), 0f, spawnInterval);
    }

    void SpawnObjectD()
    {
        if (SpawnTransform == null)
        {
            Debug.LogWarning("dSpawnTransform is not assigned!");
            return;
        }

        GameObject dInstance = Instantiate(objectPrefab, SpawnTransform.position, Quaternion.identity);
        Rigidbody2D dRb = dInstance.GetComponent<Rigidbody2D>();
        if (dRb != null)
        {
            dRb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
