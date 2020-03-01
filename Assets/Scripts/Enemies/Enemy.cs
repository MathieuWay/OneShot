using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
    public class Enemy : MonoBehaviour
    {
        public delegate void EnemyDead();
        public static event EnemyDead OnEnemyDead;

        //public pattern
        private Path path;

        private Animator anim;
        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
            path = GetComponent<Path>();
            if(path)
                path.InitPath(transform.position);
        }

        private void Update()
        {
            if(path)
                transform.position = path.GetPositionAlongPath();
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
}
