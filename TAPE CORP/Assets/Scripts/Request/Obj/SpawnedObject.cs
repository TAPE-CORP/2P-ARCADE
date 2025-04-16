using UnityEngine;
using UnityEngine.UI;

public class SpawnedObject : MonoBehaviour
{
    public SpawnableData data;
    private float timer;

    private void Start()
    {
        timer = data.timeLimit;

        // 크기 설정
        transform.localScale = new Vector3(data.size.x, data.size.y, 1f);

        // 스프라이트 설정 (있다면)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && data.sprite != null)
        {
            sr.sprite = data.sprite;
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ObjectSpawner.Instance.RemoveObject(this);
            Destroy(gameObject);
        }
    }
}
