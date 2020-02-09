using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public delegate void EnemyDead();
    public static event EnemyDead OnEnemyDead;

    private Animator anim; 
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            anim.SetTrigger("dying");
            OnEnemyDead();
        }
    }
}
