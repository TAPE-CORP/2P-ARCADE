using UnityEngine;

public class Door : MonoBehaviour
{
    public void Open()
    {
        Debug.Log("문 열림!");
        gameObject.SetActive(false); // or 애니메이션 등 처리
    }
}
