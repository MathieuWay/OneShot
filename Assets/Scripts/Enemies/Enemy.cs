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
		[SerializeField] private GameObject killParticle = null;
		[SerializeField] private Transform pivot = null;
		[SerializeField] private Transform weaponPivot = null;
		[SerializeField] private Direction defaultDirection = Direction.Left;
		public Direction _Direction { get; private set; }
		public Transform Pivot { get => pivot; }
		public Transform WeaponPivot { get => weaponPivot; }

		private Vector2 lastPos;
		public bool isAlive;
        public Animator anim;
        private Agent agent;

		public enum Direction { Left, Right }

		public void Init(float speed)
		{
			agent.speed = speed;
		}

		public float GetDirectionX()
		{
			return _Direction == Direction.Left ? -1 : 1;
		}

		private void Awake()
		{
			agent = GetComponent<Agent>();
			anim = GetComponentInChildren<Animator>();
		}

		private void Start()
        {
			isAlive = true;

			lastPos = transform.position;
			_Direction = defaultDirection;
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

			SetDirection();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //if (collision.gameObject.CompareTag("Player"))
            //{
            //    anim.SetTrigger("dying");
            //    OnEnemyDead();
            //}
        }

		private void SetDirection()
		{
			Vector2 dif = (Vector2)transform.position - lastPos;
			lastPos = transform.position;

			if (dif.x != 0)
			{
				_Direction = dif.x < 0 ? Direction.Left : Direction.Right;
			}

			pivot.rotation = Quaternion.Euler(0, _Direction == Direction.Left ? 180 : 0, 0);
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
