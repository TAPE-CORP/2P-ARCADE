using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("버튼 설정")]
    public float pressDepth = 0.2f;
    public float pressSpeed = 2f;

    [Header("문 설정")]
    public Door connectedDoor;
    public float doorLiftHeight = 2f;
    public float doorLiftSpeed = 2f;

    private Vector3 originalPos;
    private Vector3 pressedPos;
    private bool isPressed = false;

    private void Start()
    {
        originalPos = transform.position;
        pressedPos = originalPos - new Vector3(0, pressDepth, 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Box") && collision.name == "box정답박스")
        {
            isPressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Box") && collision.name == "box정답박스")
        {
            isPressed = false;
        }
    }

    private void Update()
    {
        // 버튼 눌림 애니메이션
        Vector3 targetPos = isPressed ? pressedPos : originalPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, pressSpeed * Time.deltaTime);

        // 문 올라가기
        if (connectedDoor != null)
        {
            if (isPressed)
            {
                connectedDoor.MoveUp(doorLiftHeight, doorLiftSpeed);
            }
        }
    }
}
