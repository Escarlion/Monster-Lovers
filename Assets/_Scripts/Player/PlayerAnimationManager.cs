using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private AnimationClip idle, walk, jump, dash;
    [SerializeField] private List<AnimationClip> attacks = new List<AnimationClip>();
    private Animator animator;
    private string currentAnimation;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void IdleAnimation()
    {
        ChangeAnimation(idle.name);
    }
    public void JumpAnimation()
    {
        ChangeAnimation(jump.name);
    }
    public void DashAnimation()
    {
        ChangeAnimation(dash.name);
    }
    public void WalkAnimation()
    {
        ChangeAnimation(walk.name);
    }
    private void ChangeAnimation(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        animator.Play(newAnimation);

        currentAnimation = newAnimation;

    }
}
