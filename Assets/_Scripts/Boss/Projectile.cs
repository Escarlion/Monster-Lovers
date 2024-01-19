using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float destroyAfter;
    [SerializeField] bool penetrative;
    [Tooltip("Uso de variaveis:" +
        "\n\nDirect: velocity" +
        "\n\nAceleration: velocity, aceleration rate" +
        "\n\nDelay: velocity, initial/end position, movement duration, player direction, delay wait time" +
        "\n\nBoomerang: velocity, initial/end position, movement duration, boomerang wait time" +
        "\n\nRoute: velocity, movement duration, route positions")]
    [SerializeField] ProjectileType type;

    Rigidbody2D rb;
    Transform player;

    [Header("Movement")] 
    [SerializeField] float velocity;
    [SerializeField] Vector3 initialPostion;
    [SerializeField] Vector3 endPosition;
    [SerializeField] float movementDuration;

    //Variaveis que dependem dos tipos
    //Aceleration
    [Header("Aceleration")]
    [SerializeField] float acelerationRate;
    float actualAceleration;

    //Delay
    [Header("Delay")]

    [SerializeField] bool playerDirection;
    [SerializeField] float delayWaitTime;

    //Boomerang
    [Header("Boomerang")]

    [SerializeField] float boomerangWaitTime;

    //Route
    [Header("Route")]

    [SerializeField] List<Vector3> routePositions = new List<Vector3>();


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<Player>().transform;

        TypeSelect();

        Destroy(gameObject, destroyAfter);
    }

    private void TypeSelect()
    {
        switch (type)
        {
            case ProjectileType.Direct:
                DirectProject();
                break;
            case ProjectileType.Aceleration:
                AcelerationProject();
                break;
            case ProjectileType.Delay:
                StartCoroutine(DelayProject());
                break;
            case ProjectileType.Boomerang:
                StartCoroutine(BoomerangProject());
                break;
            case ProjectileType.Route:
                StartCoroutine(RouteProject());
                break;

        }
    }
    private void DirectProject()
    {
        rb.AddForce(Vector2.right * velocity, ForceMode2D.Impulse);
    }
    private void AcelerationProject()
    {
        rb.velocity = new Vector2(0.1f, rb.velocity.y);
        StartCoroutine(Aceleration());
    }

    IEnumerator Aceleration()
    {
        while (rb.velocity.x < velocity)
        {
            rb.velocity = new Vector2(rb.velocity.x * acelerationRate, rb.velocity.y);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator DelayProject()
    {

        StartCoroutine(GoTo(initialPostion));

        yield return new WaitForSeconds(delayWaitTime);

        if (playerDirection) ImpulseToPlayerPosition();
        else StartCoroutine(GoTo(endPosition));

    }

    IEnumerator BoomerangProject()
    {
        Debug.Log("Boomerang iniciado");
        transform.position = initialPostion;

        Debug.Log("partiu");
        StartCoroutine(GoTo(endPosition));
        Debug.Log("chegou");

        yield return new WaitForSeconds(boomerangWaitTime);

        StartCoroutine(GoTo(initialPostion));
    }

    IEnumerator RouteProject()
    {
        foreach(Vector3 position in routePositions)
        {
            StartCoroutine(GoTo(position));
            yield return new WaitForSeconds(movementDuration);
        }
    }

    private IEnumerator GoTo(Vector3 newLocation)
    {
        float time = 0f;

        while (time < movementDuration)
        {
            transform.position = Vector3.Lerp(transform.position, newLocation, time / movementDuration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void ImpulseToPlayerPosition()
    {
        Vector2 direction = player.position - transform.position;
        direction.Normalize();
        rb.AddForce(direction * velocity, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().TakeDamage();
            if (!penetrative) Destroy(gameObject);
        }
    }


    enum ProjectileType
    {
        Direct,
        Aceleration,
        Delay,
        Boomerang,
        Route,
    }

}
