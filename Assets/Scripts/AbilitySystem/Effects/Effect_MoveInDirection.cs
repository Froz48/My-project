using System;
using UnityEngine;

public class Effect_MoveInDirection : MonoBehaviour
{
    public Vector2 direction;
    public float speed;

    public void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;
    }
}