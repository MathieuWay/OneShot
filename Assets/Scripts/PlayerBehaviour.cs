using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public class PlayerBehaviour : MonoBehaviour
	{
		public static PlayerBehaviour Instance { get; private set; }

		[SerializeField] private Transform centerPivot = null;
		public Transform CenterPivot { get => centerPivot; }
		public bool IsDead { get; private set; }
		private Animator anim;

		public void Kill()
		{
			if (IsDead) return;

			IsDead = true;

			anim.SetTrigger("dying");

			LevelController.Instance.PlayerDie();
		}

		private void Awake()
		{
			if(Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}

		private void Start()
		{
			anim = GetComponentInChildren<Animator>();
		}
	}
}