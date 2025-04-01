<<<<<<< HEAD
using UnityEngine;

public class CorrectBoxTrigger : MonoBehaviour
{
    [Header("¹®Æ² ÀÚ½Ä ¿ä¼Ò")]
    public Transform doorButton;
    public Transform door;

    [Header("ÀÌµ¿ ¼³Á¤")]
=======
ï»¿using UnityEngine;

public class CorrectBoxTrigger : MonoBehaviour
{
    [Header("ë¬¸í‹€ ìžì‹ ìš”ì†Œ")]
    public Transform doorButton;
    public Transform door;

    [Header("ì´ë™ ì„¤ì •")]
>>>>>>> main
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
<<<<<<< HEAD
        if (other.name.Contains("boxÁ¤´ä¹Ú½º"))
=======
        if (other.name.Contains("boxì •ë‹µë°•ìŠ¤"))
>>>>>>> main
        {
            isCorrectBoxAbove = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
<<<<<<< HEAD
        if (other.name.Contains("boxÁ¤´ä¹Ú½º"))
=======
        if (other.name.Contains("boxì •ë‹µë°•ìŠ¤"))
>>>>>>> main
        {
            isCorrectBoxAbove = false;
        }
    }
}