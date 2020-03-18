using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
	public class NinjaEnemy : EnemyBase
	{
		public override void Hit(AttackName attackName, Vector2 hitOriginPos)
		{
			if (attackName != AttackName.SlashAttack)
			{
				PlayerAttackFailed();
				return;
			}

			base.Hit(attackName, hitOriginPos);
		}
	}
}
