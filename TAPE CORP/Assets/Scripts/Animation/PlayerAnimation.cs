using UnityEngine;

public class PlayerAnimatorController : PlayerController
{
    [System.Serializable]
    public class PartAnimation
    {
        public string partName;
        public Animation animationComponent;
        public AnimationClip idleClip;
        public AnimationClip moveClip;
        public AnimationClip fallClip;
    }

    [Header("¹ÙÄû")]
    public PartAnimation wheel1;
    public PartAnimation wheel2;

    [Header("´«")]
    public PartAnimation eye1;
    public PartAnimation eye2;

    [Header("¸öÅë")]
    public PartAnimation body;

    [Header("¸ðÀÚ")]
    public PartAnimation hat;

    public enum PlayerState
    {
        Idle,
        Moving,
        Falling
    }

    public PlayerState currentState = PlayerState.Idle;

    public override void Update()
    {
        base.Update();
        UpdateState();

        UpdateAnimation(wheel1);
        UpdateAnimation(wheel2);
        UpdateAnimation(eye1);
        UpdateAnimation(eye2);
        UpdateAnimation(body);
        UpdateAnimation(hat);
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

    private void UpdateAnimation(PartAnimation part)
    {
        if (part == null || part.animationComponent == null) return;

        AnimationClip clip = GetClipForState(part);
        if (clip == null) return;

        if (part.animationComponent.clip != clip)
        {
            part.animationComponent.clip = clip;
        }

        if (!part.animationComponent.IsPlaying(clip.name))
        {
            part.animationComponent.Play();
        }
    }

    private AnimationClip GetClipForState(PartAnimation part)
    {
        switch (currentState)
        {
            case PlayerState.Moving: return part.moveClip;
            case PlayerState.Falling: return part.fallClip;
            case PlayerState.Idle:
            default: return part.idleClip;
        }
    }
}
