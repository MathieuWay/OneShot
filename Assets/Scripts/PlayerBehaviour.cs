﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public class PlayerBehaviour : MonoBehaviour
	{
		public static PlayerBehaviour Instance { get; private set; }

		[SerializeField] private Transform centerPivot = null;
		[SerializeField] private GameObject pivot = null;
		[SerializeField] private Animator chronaAnim = null;
		public Transform CenterPivot { get => centerPivot; }
		public bool IsDead { get; private set; }
		private Animator anim;

		public void Kill()
		{
			if (IsDead) return;

			IsDead = true;

			//!TMP
			pivot.SetActive(false);

			anim.Play("dying");

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

			ComboController.Instance.ComboSuccessEvent += PlayAnim;
		}

		private void PlayAnim(AttackName attackName)
		{
			switch(attackName)
			{
				case AttackName.SpiralAttack:
					chronaAnim.SetTrigger("Spiral");
					break;

				case AttackName.SlashAttack:
					chronaAnim.SetTrigger("Slash");
					break;

				case AttackName.ThrustAttack:
					chronaAnim.SetTrigger("Thrust");
					break;
			}
		}
	}
}