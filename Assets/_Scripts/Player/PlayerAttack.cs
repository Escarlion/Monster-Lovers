using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float bounceForce;

    Player player;
    Rigidbody2D rb;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void Bounce()
    {
        rb.velocity = new Vector2(rb.velocity.x, bounceForce);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Dano no inimigo");
        }
        if (collision.CompareTag("Jumpable") && player.actualState == Player.MovementState.JumpAttack)
        {
            Debug.Log("Bounce");
            Bounce();
        }
    }
}
