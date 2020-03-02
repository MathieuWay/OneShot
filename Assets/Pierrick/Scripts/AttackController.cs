using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public enum AttackName { SpiralAttack, ThrustAttack, SlashAttack }


	public class AttackController : MonoBehaviour
	{
		public static AttackController Instance { get; private set; }

		[SerializeField] private Attack[] attacks;
		private Dictionary<AttackName, Attack> attacksDic;

		public void LaunchAttack(AttackName attackName)
		{
			if (!attacksDic[attackName].IsReady) return;

			attacksDic[attackName].Launch();

			StartCoroutine(RepeatAttackDelay(attackName));
		}


		private void Awake()
		{
			if(Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;

			attacksDic = new Dictionary<AttackName, Attack>();
			for (int i = 0; i < attacks.Length; i++)
			{
				attacksDic.Add(attacks[i]._AttackName, attacks[i]);
			}
		}

		private IEnumerator RepeatAttackDelay(AttackName attackName)
		{
			attacksDic[attackName].IsReady = false;
			yield return new WaitForSeconds(attacksDic[attackName].RepeatDelay);
			attacksDic[attackName].IsReady = true;
		}
	}
}