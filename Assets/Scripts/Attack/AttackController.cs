﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public enum AttackName { SpiralAttack, ThrustAttack, SlashAttack }


	public class AttackController : MonoBehaviour
	{
		public static AttackController Instance { get; private set; }

		[SerializeField] private Attack[] attacks = null;
		private Dictionary<AttackName, Attack> attacksDic;

		public void LaunchAttack(AttackName attackName)
		{
			if (LevelController.Instance.phase != Phase.Combat) return;

			attacksDic[attackName].Launch();
		}

		public void DisplayAttack(AttackName attackName)
		{
			if (LevelController.Instance.phase != Phase.Combat) return;

			ComboController.Instance.DisplayCombo(attacksDic[attackName].Combo);
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
			attacksDic = new Dictionary<AttackName, Attack>();
			List<Combo> combos = new List<Combo>();

			for (int i = 0; i < attacks.Length; i++)
			{
				attacksDic.Add(attacks[i]._AttackName, attacks[i]);

				attacks[i].Combo.Init(attacks[i]._AttackName);
				combos.Add(attacks[i].Combo);
			}

			ComboController.Instance.ComboSuccessEvent += LaunchAttack;

			StartCoroutine(StartDelay(combos.ToArray()));
		}

		private IEnumerator StartDelay(Combo[] combos)
		{
			yield return new WaitForEndOfFrame();
			ComboController.Instance.StartCombos(combos);
		}
	}
}