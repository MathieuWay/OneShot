using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
	public abstract class EnemyBase : MonoBehaviour
	{
		[SerializeField] protected EnemyData enemyData;

		private void Start()
		{
			GetComponent<Enemy>().Init(enemyData.Speed);
		}

		public virtual void Hit(AttackName attackName, Vector2 hitOriginPos)
		{
			GetComponent<Enemy>().Kill();
		}
	}
}