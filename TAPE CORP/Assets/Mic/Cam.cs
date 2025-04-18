using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public Transform player; // 따라갈 플레이어
    public Vector3 offset;   // 카메라의 플레이어에 대한 상대적 위치

    void LateUpdate()
    {
        if (player != null)
            transform.position = player.position + offset;
    }
}
