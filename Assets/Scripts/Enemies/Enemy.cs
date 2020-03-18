using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
    public class Enemy : MonoBehaviour
    {
        public delegate void EnemyDead();
        public static event EnemyDead OnEnemyDead;
		public event EnemyDead OnKill;

		//public pattern
		//private Path path;
		[SerializeField] private GameObject killParticle;
		public bool isAlive;
        public Animator anim;
        private Agent agent;

		public void Init(float speed)
		{
			agent.speed = speed;
		}

		private void Awake()
		{
			agent = GetComponent<Agent>();
		}

		private void Start()
        {
			isAlive = true;
            anim = GetComponentInChildren<Animator>();
            
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
			if (!isAlive) return;

			isAlive = false;
			anim.SetTrigger("dying");
			OnEnemyDead();

			CameraShake.Instance.ShakeCamera();
			Gamepad.Instance.Vibrate(0.5f, 0.5f, 0.5f);

			//FX
			Instantiate(killParticle, transform.position + new Vector3(0, 0.2f, 0), killParticle.transform.rotation);

			OnKill?.Invoke();
		}
	}
}
