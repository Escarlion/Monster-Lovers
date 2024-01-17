using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    [SerializeField] protected int life;
    [SerializeField] protected float attackWaitTime;

    protected bool isDead, isAttacking;

    protected int stageCount;
    [SerializeField]protected int actualStage = 1;

    protected List<string> attackList = new List<string>();
    protected List<string> conditionalAttacks = new List<string>();
    [SerializeField] protected List<AnimationClip> attackAnimations = new List<AnimationClip>();
    
    Animator animator;
    private string currentAnimation;

    protected abstract void PrepareAtack();
    protected abstract string GetAttack();
    protected abstract void FillAttackList(int actualStage);
    protected abstract void FillConditionalAttacks();

    protected abstract bool CheckConditions(string attack);
    
    public void TakeDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            if(actualStage < stageCount)
            {
                actualStage++;
                //run cutscene
            }
            else
            {
                isDead = true;
                //run death
            }
        }
        Debug.Log("Vida do boss: " + life);
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void ChangeAnimation(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        animator.Play(newAnimation);

        currentAnimation = newAnimation;

    }

    protected bool BossDistance(Transform desiredObject, float desiredDistance)
    {
        float distance = Vector2.Distance(desiredObject.position, transform.position);
        Debug.Log("distance = " + distance);
        if (distance < desiredDistance) return true;
        else return false;
    }

}
