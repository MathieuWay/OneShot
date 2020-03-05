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
		//private Path path;
		public bool isAlive;
        public Animator anim;
        private Agent agent;

        private void Start()
        {
			isAlive = true;
            anim = GetComponentInChildren<Animator>();
            agent = GetComponent<Agent>();
            /*path = GetComponent<Path>();
            if(path)
                path.InitPath(transform.position);*/
        }

        private void Update()
        {
			anim.speed = GameTime.Instance.TimeSpeed;
            /*if(path)
                transform.position = path.GetPositionAlongPath();*/
            if (agent)
            {
                if (!agent.reach)
                    anim.SetBool("isMoving", true);
                else
                    anim.SetBool("isMoving", false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //if (collision.gameObject.CompareTag("Player"))
            //{
            //    anim.SetTrigger("dying");
            //    OnEnemyDead();
            //}
        }

		public void Kill()
		{
			isAlive = false;
			anim.SetTrigger("dying");
			OnEnemyDead();
		}
	}
}
