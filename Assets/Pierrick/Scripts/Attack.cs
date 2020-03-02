using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public abstract class Attack : MonoBehaviour
	{
		[SerializeField] private AttackName attackName;
		[SerializeField] private float repeatDelay = 2;
		public AttackName _AttackName { get => attackName; }
		public float RepeatDelay { get => repeatDelay; }
		public bool IsReady { get; set; }

#if UNITY_EDITOR
		private bool isDebug;
		private float detectionRange;
#endif

		public abstract void Launch();

		protected virtual void Start()
		{
			IsReady = true;
		}

		protected Enemy GetNearestEnemy(float detectionRange)
		{
			Collider2D[] colliders = Physics2D.OverlapBoxAll(PlayerBehaviour.Instance.CenterPivot.position, new Vector2(detectionRange, 1), 0);

#if UNITY_EDITOR
			StartCoroutine(DebugOverlapBox(1, detectionRange));
#endif

			List<Transform> enemies = new List<Transform>();

			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].CompareTag("Enemy"))
				{
					enemies.Add(colliders[i].transform);
				}
			}

			float minDistance = detectionRange;
			Transform target = null;

			for (int i = 0; i < enemies.Count; i++)
			{
				float distance = Vector2.Distance(PlayerBehaviour.Instance.CenterPivot.position, enemies[i].position);

				if (distance < minDistance)
				{
					minDistance = distance;
					target = enemies[i];
				}
			}

			Enemy enemy = null;

			if(target != null)
			{
				enemy = target.GetComponent<Enemy>();
				Debug.Log("HIT: " + target.name + " " + target.position);
			}

			return enemy;
		}

#if UNITY_EDITOR
		private IEnumerator DebugOverlapBox(float duration, float detectionRange)
		{
			isDebug = true;
			this.detectionRange = detectionRange;
			yield return new WaitForSeconds(duration);
			isDebug = false;
		}

		private void OnDrawGizmos()
		{
			if(isDebug)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(PlayerBehaviour.Instance.CenterPivot.position, new Vector3(detectionRange, 1, 0));
			}
		}
#endif
	}
}