// Conveyor.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Conveyor : MonoBehaviour
{
    [Tooltip("컨베이어 벨트의 속도")]
    public float speed = 1.5f;
    [Tooltip("컨베이어 진행 방향 플래그 (회전 토글용)")]
    public bool isRight = true;

    private Vector2 moveDirection;
    private float _originalZRotation;

    void Awake()
    {
        // 원본 Z 회전 저장
        _originalZRotation = transform.localEulerAngles.z;
        ApplyFlip();

        // 트리거로 설정
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        // 항상 transform.right 방향 그대로 사용
        var dir3 = transform.right.normalized;
        moveDirection = new Vector2(dir3.x, dir3.y);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Translate 방식으로 밀어주기 (플레이어 이동과 자연스럽게 합산됨)
        collision.transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    public void SetDirection()
    {
        // 플래그 토글 → 180도 회전
        isRight = !isRight;
        ApplyFlip();
    }

    private void ApplyFlip()
    {
        float targetZ = isRight
            ? _originalZRotation
            : _originalZRotation + 180f;
        transform.localEulerAngles = new Vector3(0, 0, targetZ);
    }
}
