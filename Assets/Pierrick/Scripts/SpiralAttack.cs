﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class SpiralAttack : Attack
	{
		[SerializeField] private float radius = 1;

#if UNITY_EDITOR
		private bool isDebug;
#endif

		public override void Launch()
		{
			Debug.Log("HEY");

			Collider2D[] hits = Physics2D.OverlapCircleAll(PlayerBehaviour.Instance.CenterPivot.position, radius);

#if UNITY_EDITOR
			StartCoroutine(DrawAttack(1));
#endif

			for (int i = 0; i < hits.Length; i++)
			{
				if(hits[i].CompareTag("Enemy"))
				{
					Debug.Log("HIT: " + hits[i].name + " " + Vector2.Distance(hits[i].transform.position, PlayerBehaviour.Instance.CenterPivot.position));

					hits[i].GetComponent<Enemy>().Kill();
				}
			}
		}

#if UNITY_EDITOR
		private IEnumerator DrawAttack(float duration)
		{
			isDebug = true;
			yield return new WaitForSeconds(duration);
			isDebug = false;
		}

		private void OnDrawGizmos()
		{
			if (isDebug)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(PlayerBehaviour.Instance.CenterPivot.position, radius);
			}
		}
#endif
	}
}