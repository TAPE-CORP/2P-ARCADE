using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("��ư ����")]
    public float pressDepth = 0.2f;
    public float pressSpeed = 2f;

    [Header("�� ����")]
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
        if (collision.CompareTag("Box") && collision.name == "box����ڽ�")
        {
            isPressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Box") && collision.name == "box����ڽ�")
        {
            isPressed = false;
        }
    }

    private void Update()
    {
        // ��ư ���� �ִϸ��̼�
        Vector3 targetPos = isPressed ? pressedPos : originalPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, pressSpeed * Time.deltaTime);

        // �� �ö󰡱�
        if (connectedDoor != null)
        {
            if (isPressed)
            {
                connectedDoor.MoveUp(doorLiftHeight, doorLiftSpeed);
            }
        }
    }
}
