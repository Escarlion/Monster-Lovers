using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTest : MonoBehaviour
{
    [SerializeField] GameObject direct, aceleration, delay, boomerang, route;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Instantiate(direct);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Instantiate(aceleration);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Instantiate(delay);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Instantiate(boomerang);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Instantiate(route);
        }
        
    }
}
