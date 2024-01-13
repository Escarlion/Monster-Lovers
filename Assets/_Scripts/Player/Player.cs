using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerAnimationManager animationManager;

    [SerializeField] float speed = 5f; // Velocidade de movimentação do jogador
    [SerializeField] float jumpForce = 3.5f; // Força do pulo
    [SerializeField] float dashTime = 1f;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator animator;
    private SpriteRenderer sr;

    [SerializeField] private LayerMask groundLayer;

    int comboCount;
    [SerializeField] private bool isAttacking = false;

    void Start()
    {
        animationManager = GetComponent<PlayerAnimationManager>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Movimentação horizontal
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animationManager.JumpAnimation();
        }
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //if (!stateInfo.IsName("Attack01") || !stateInfo.IsName("Attack02") || !stateInfo.IsName("Attack03")) FinishCombo();

        MovementState state;

        //Dash
        if (Input.GetButtonDown("Dash"))
        {
            state = MovementState.Dashing;
            animationManager.DashAnimation();
            Debug.Log("Dash");
        }

        //if (isAttacking) return;

        //Andar
        if (rb.velocity.x > 0)
        {
            state = MovementState.Walking;
            animationManager.WalkAnimation();
            sr.flipX = false;
        }
        else if (rb.velocity.x < 0)
        {
            state = MovementState.Walking;
            animationManager.WalkAnimation();
            sr.flipX = true;
        }
        else
        {
            state = MovementState.Idle;
        }

        //pulo
        if (rb.velocity.y > .1f)
        {
            state = MovementState.Jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.Falling;
        }

        //Combo
        if (Input.GetButtonDown("Attack") && !isAttacking)
        {
            Debug.Log("Atacou");
            isAttacking = true;
            switch (comboCount)
            {
                case 0:
                    state = MovementState.Attack01;
                    break;
                case 1:
                    state = MovementState.Attack02;
                    break;
                case 2:
                    state = MovementState.Attack03;
                    break;
            }
        }

        animator.SetInteger("state", ((int)state));
    }

    //Verifica se o player está em contato com o chão
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, groundLayer);
    }

    //Ataques

    //chamado pela timeline para possibilitar o proximo combo
    private void StartCombo()
    {
        isAttacking = false;
        if (comboCount < 3)
        {
            comboCount++;
        }
    }

    //chamado pela timeline para resetar o combo
    private void FinishCombo()
    {
        isAttacking = false;
        comboCount = 0;
    }

    private enum MovementState
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Dashing,
        Attack01,
        Attack02,
        Attack03,
    }
}
