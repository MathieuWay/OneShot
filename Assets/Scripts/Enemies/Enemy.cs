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
		[SerializeField] private GameObject killParticle;
		public bool isAlive;
        public Animator anim;
        public float speed = 1f;
        //private Agent agent;
        private Vector3 initialPosition;
        private Pattern pattern;

        private void Start()
        {
			isAlive = true;
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

			//FX
			Instantiate(killParticle, transform.position + new Vector3(0, 0.2f, 0), killParticle.transform.rotation);
		}

        public void ResetAgent()
        {
            isAlive = true;
            anim.SetBool("isMoving", false);
            anim.Rebind();
            transform.position = initialPosition;
            //currentLayer = LayersController.instance.GetLayer(LayersController.instance.GetLayerIndexByHeight(transform.position.y));
        }
    }
}
