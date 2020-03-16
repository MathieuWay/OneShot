using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
	public class ShieldedEnemy : EnemyBase
	{
		[Header("TMP: Solution temporaire pour indiquer la direction de l'ennemi")]
		[SerializeField] private float xDir;

		public override void Hit(AttackName attackName, Vector2 hitOriginPos)
		{
			Vector2 dirToOrigin = (hitOriginPos - (Vector2)transform.position).normalized;

			Debug.Log("DIR: " + Vector2.Dot(dirToOrigin, new Vector2(xDir, 0)));

			//Si les deux vecteurs pointent dans la même direction (la frappe ne vient pas de derrière), il ne prend pas de dégât
			if (Vector2.Dot(dirToOrigin, new Vector2(xDir, 0)) > 0) return;

			base.Hit(attackName, hitOriginPos);
		}
	}
}
