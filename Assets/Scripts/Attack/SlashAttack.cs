using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public class SlashAttack : Attack
	{
		[Header("SLASH PARAMETERS")]
		[SerializeField] private float detectionRange = 1;

		public override void Launch()
		{
			Enemy enemy = GetNearestEnemy(new Vector2(1, detectionRange)); 

			if (enemy != null)
			{
				if (enemy.GetComponent<EnemyBase>() != null)
				{
					enemy.GetComponent<EnemyBase>().Hit(_AttackName, PlayerBehaviour.Instance.CenterPivot.position);
				}
				else
				{
					enemy.Kill();
				}
			}
		}
	}
}