using UnityEngine;

public class Door : MonoBehaviour
{
    public void Open()
    {
        Debug.Log("�� ����!");
        gameObject.SetActive(false); // or �ִϸ��̼� �� ó��
    }
}
