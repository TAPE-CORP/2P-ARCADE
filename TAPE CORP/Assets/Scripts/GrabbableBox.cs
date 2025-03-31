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
            Debug.Log("¹Ú½º ÀâÈû!");

       
        }
    }
}
