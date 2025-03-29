using UnityEngine;

public class PlayerAnimatorController : PlayerController
{
    [Header("애니메이션 대상")]
    public SpriteRenderer wheelRenderer;
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer hatRenderer;
    public SpriteRenderer eyesRenderer;

    public enum PlayerState
    {
        Idle,
        Moving,
        Falling
    }

    public PlayerState currentState = PlayerState.Idle;

    public  override void Update()
    {
        base.Update();

        UpdateState();
        UpdateAnimation();
    }

    private void UpdateState()
    {
        if (!isGrounded)
        {
            currentState = PlayerState.Falling;
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            currentState = PlayerState.Moving;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    private void UpdateAnimation()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                // 예시: 눈 깜빡임 애니메이션
                eyesRenderer.flipY = false;
                break;
            case PlayerState.Moving:
                // 예시: 바퀴 회전 시각화
                wheelRenderer.transform.Rotate(Vector3.forward, -360 * Time.deltaTime);
                break;
            case PlayerState.Falling:
                bodyRenderer.transform.localRotation = Quaternion.Euler(0, 0, 15f);
                break;
        }
    }
}
