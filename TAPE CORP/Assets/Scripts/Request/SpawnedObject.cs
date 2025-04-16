using UnityEngine;
using UnityEngine.UI;

public class SpawnedObject : MonoBehaviour
{
    public SpawnableData data;
    private float timer;

    private void Start()
    {
        timer = data.timeLimit;

        // ũ�� ����
        transform.localScale = new Vector3(data.size.x, data.size.y, 1f);

        // ��������Ʈ ���� (�ִٸ�)
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
