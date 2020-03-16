using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
	public class BulldozerEnemy : EnemyBase
	{
		public override void Hit(AttackName attackName, Vector2 hitOriginPos)
		{
			if (attackName != AttackName.ThrustAttack) return;

			base.Hit(attackName, hitOriginPos);
		}
	}
}
