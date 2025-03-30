using UnityEngine;

public class GrabbableBox : MonoBehaviour
{
    public Door targetDoor;
    private bool isGrabbed = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isGrabbed) return;

        if (collision.CompareTag("Player") && Input.GetKey(KeyCode.Space))
        {
            isGrabbed = true;
            Debug.Log("박스 잡힘!");

            // 예시: 문 열기
            if (targetDoor != null)
            {
                targetDoor.Open();
            }
        }
    }
}
