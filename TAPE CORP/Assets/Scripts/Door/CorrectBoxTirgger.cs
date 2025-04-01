using UnityEngine;

public class CorrectBoxTrigger : MonoBehaviour
{
    [Header("문틀 자식 요소")]
    public Transform doorButton;
    public Transform door;

    [Header("이동 설정")]
    public float moveAmount = 0.1f;
    public float moveSpeed = 5f;

    private Vector3 buttonInitialLocalPos;
    private Vector3 doorInitialLocalPos;
    private bool isCorrectBoxAbove = false;

    void Start()
    {
        buttonInitialLocalPos = doorButton.localPosition;
        doorInitialLocalPos = door.localPosition;
    }

    void Update()
    {
        Vector3 targetButtonPos = buttonInitialLocalPos;
        Vector3 targetDoorPos = doorInitialLocalPos;

        if (isCorrectBoxAbove)
        {
            targetButtonPos.y -= moveAmount;
            targetDoorPos.y += moveAmount;
        }

        doorButton.localPosition = Vector3.Lerp(doorButton.localPosition, targetButtonPos, Time.deltaTime * moveSpeed);
        door.localPosition = Vector3.Lerp(door.localPosition, targetDoorPos, Time.deltaTime * moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Contains("box정답박스"))
        {
            isCorrectBoxAbove = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name.Contains("box정답박스"))
        {
            isCorrectBoxAbove = false;
        }
    }
}