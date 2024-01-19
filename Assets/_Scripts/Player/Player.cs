using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] int maxLife = 5;
    int actualLife;

    //Movimentação
    [SerializeField] float speed = 5f;

    [SerializeField] float jumpForce = 3.5f;
    [SerializeField] private float jumpForceLimit = 2f;
    [SerializeField] private float _jumpForceMultiplier = 1f;
    private float jumpForceMultiplier = 1f;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private bool isJumping = false;
    private float fallTime = 0.65f;
    private float fallMultiplier;

    //Dash
    [SerializeField] private float dashTime;
    [SerializeField] private float dashPower;
    [SerializeField] private float dashCooldown;
    private bool isDashing = false;
    private bool canDash = true;

    //Objetos
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform attackHitbox;
    PlayerAnimationManager animationManager;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator animator;
    private SpriteRenderer sr;
    [HideInInspector] public MovementState actualState;

    //Combo
    private bool isAttacking = false;
    int comboCount;

    void Start()
    {
        animationManager = GetComponent<PlayerAnimationManager>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        actualLife = maxLife;
    }
    private void FixedUpdate()
    {
        if (isAttacking) return;
        if (isDashing) return;

        // Movimentação horizontal

        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if (rb.velocity.x > 0)
        {
            sr.flipX = false;
            attackHitbox.rotation = Quaternion.Euler(0f, -0, 0f);
        }
        else if (rb.velocity.x < 0)
        {
            sr.flipX = true;
            attackHitbox.rotation = Quaternion.Euler(0f, -180f, 0f);
        }

    }
    void Update()
    {
        if (isDashing) return;

        //controladores dos timers do coyotefall e da queda lenta
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            fallMultiplier = fallTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        //aumenta a força do jogador conforme ele segura o botão, até certo limite
        if(Input.GetButton("Jump") && IsGrounded())
        {
            if (jumpForceMultiplier < jumpForceLimit) jumpForceMultiplier += Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump") && coyoteTimeCounter > 0f && !isAttacking)
        {
            //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            rb.AddForce(Vector2.up * jumpForce * jumpForceMultiplier, ForceMode2D.Impulse);
            animationManager.JumpAnimation();
            jumpForceMultiplier = _jumpForceMultiplier;
            isJumping = true;
        }
        if(isJumping && rb.velocity.y == 0f)
        {
            isJumping = false;
        }
        //diminui a velocidade do jogador quando chega no pico do pulo
        if (isJumping && rb.velocity.y < 0f)
        {
            if (IsGrounded())
            {
                rb.gravityScale = 1f;
                return;
            }
            rb.gravityScale = fallMultiplier;
            if(fallMultiplier < 1)
            {
                fallMultiplier += Time.deltaTime;
            }
            else
            {
                fallMultiplier = 1;
            }
        }

        //Dash
        if (Input.GetButtonDown("Dash") && canDash)
        {
            //Debug.Log("Dash");
            StartCoroutine(Dash());
        }
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (isAttacking) return;

        MovementState state = MovementState.Idle;
        //Andar
        if (rb.velocity.x > 1 || rb.velocity.x < -1)
        {
            if (!isDashing && IsGrounded() && !isAttacking) state = MovementState.Walking;
        }
        else if (isDashing)
        {
            state = MovementState.Dashing;
        }
        else if (rb.velocity.x == 0 && !isDashing && !isAttacking)
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
            rb.velocity = new Vector2(0f,rb.velocity.y);
            state = ExecuteCombo();
        }

        actualState = state;
        animator.SetInteger("state", (int)state);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        animator.SetBool("isDashing", isDashing);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        if (sr.flipX == true) rb.velocity = new Vector2(transform.localScale.x * -dashPower, 0f);
        else rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
        animator.SetBool("isDashing", isDashing);

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
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

    //escolhe o proximo ataque e coloca a animação correta
    private MovementState ExecuteCombo()
    {
        //Debug.Log("Atacou");
        MovementState state;
        isAttacking = true;

        if (!IsGrounded())
        {
            state = MovementState.JumpAttack;
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        else
        {
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
                default:
                    state = MovementState.Idle;
                    break;
            }
        }

        return state;
    }

    public void TakeDamage()
    {
        actualLife--;

        Debug.Log("Player perdeu vida.\nVida atual: "+actualLife);

        if(actualLife <= 0)
        {
            Debug.Log("Player morreu");
            //sequencia de morte
        }
        
    }

    public enum MovementState
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Dashing,
        Attack01,
        Attack02,
        Attack03,
        JumpAttack,
    }
}
