using UnityEngine;

public class PlayerAnimatorController : PlayerController
{
    [Header("�ִϸ��̼� ���")]
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
                // ����: �� ������ �ִϸ��̼�
                eyesRenderer.flipY = false;
                break;
            case PlayerState.Moving:
                // ����: ���� ȸ�� �ð�ȭ
                wheelRenderer.transform.Rotate(Vector3.forward, -360 * Time.deltaTime);
                break;
            case PlayerState.Falling:
                bodyRenderer.transform.localRotation = Quaternion.Euler(0, 0, 15f);
                break;
        }
    }
}
