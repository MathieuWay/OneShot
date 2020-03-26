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
        public float speed = 1f;
        //private Agent agent;
        private Vector3 initialPosition;
        private Pattern pattern;

		public enum Direction { Left, Right }

		public float GetDirectionX()
		{
			return _Direction == Direction.Left ? -1 : 1;
		}

		private void Awake()
		{
			anim = GetComponentInChildren<Animator>();
		}

		private void Start()
        {
			isAlive = true;
			lastPos = transform.position;
			_Direction = defaultDirection;
            pattern = GetComponent<Pattern>();
            anim = GetComponentInChildren<Animator>();
            //agent = GetComponent<Agent>();
            initialPosition = transform.position;
            /*path = GetComponent<Path>();
            if(path)
                path.InitPath(transform.position);*/
        }

        private void Update()
        {
			anim.speed = GameTime.Instance.TimeSpeed;
            /*if(path)
                transform.position = path.GetPositionAlongPath();*/
            /*if (agent)
            {
                if (!agent.reach)
                    anim.SetBool("isMoving", true);
                else
                    anim.SetBool("isMoving", false);
            }*/
            if (pattern)
            {
                if (pattern.currentStep != null)
                {
                    if(pattern.currentStep.type == StepType.Move)
                        anim.SetBool("isMoving", true);
                    else
                        anim.SetBool("isMoving", false);
                }
                else
                {
                    anim.SetBool("isMoving", false);
                }
            }
            else
            {
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
			anim.Play("dying");
			OnEnemyDead();

			//!TMP
			pivot.gameObject.SetActive(false);

			CameraShake.Instance.ShakeCamera();
			Gamepad.Instance.Vibrate(0.5f, 0.5f, 0.5f);

			//FX
			Instantiate(killParticle, transform.position + new Vector3(0, 0.2f, 0), killParticle.transform.rotation);

			OnKill?.Invoke();
		}

        public void ResetEnemy()
        {
            isAlive = true;

			if(anim)
			{
				anim.SetBool("isMoving", false);
				anim.Rebind();

				transform.position = initialPosition;
			}
            
            //currentLayer = LayersController.instance.GetLayer(LayersController.instance.GetLayerIndexByHeight(transform.position.y));
        }
    }
}
