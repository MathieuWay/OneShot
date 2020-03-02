using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class ThrustAttack : Attack
	{
		[SerializeField] private float detectionRange = 1;

		public override void Launch()
		{
			Enemy enemy = GetNearestEnemy(detectionRange);

			if (enemy != null)
			{
				enemy.Kill();
			}
		}
	}
}