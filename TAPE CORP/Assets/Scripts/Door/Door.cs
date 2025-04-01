using UnityEngine;

public class Door : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 targetPos;
    private bool shouldMove = false;
    private float moveSpeed = 2f;

    private void Start()
    {
        originalPos = transform.position;
        targetPos = originalPos;
    }

    public void MoveUp(float height, float speed)
    {
        targetPos = originalPos + new Vector3(0, height, 0);
        moveSpeed = speed;
        shouldMove = true;
    }

    private void Update()
    {
        if (shouldMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
                shouldMove = false;
        }
    }
}
