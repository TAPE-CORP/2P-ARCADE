using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectPrefabs; // 배열로 변경
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
            Debug.LogWarning("SpawnTransform is not assigned!");
            return;
        }

        if (objectPrefabs == null || objectPrefabs.Length == 0)
        {
            Debug.LogWarning("objectPrefabs array is empty!");
            return;
        }

        // 무작위로 하나 선택
        int randomIndex = Random.Range(0, objectPrefabs.Length);
        GameObject selectedPrefab = objectPrefabs[randomIndex];

        GameObject dInstance = Instantiate(selectedPrefab, SpawnTransform.position, Quaternion.identity);
        Rigidbody2D dRb = dInstance.GetComponent<Rigidbody2D>();
        if (dRb != null)
        {
            dRb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
