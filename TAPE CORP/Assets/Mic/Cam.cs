using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public Transform player; // ���� �÷��̾�
    public Vector3 offset;   // ī�޶��� �÷��̾ ���� ����� ��ġ

    void LateUpdate()
    {
        if (player != null)
            transform.position = player.position + offset;
    }
}
