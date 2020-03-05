using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public abstract class Attack : MonoBehaviour
	{
		[Header("MAIN")]
		[SerializeField] private AttackName attackName;
		[SerializeField] private Combo combo;
		public AttackName _AttackName { get => attackName; }
		public Combo Combo { get => combo; }

#if UNITY_EDITOR
		private bool isDebugRange;
		private float debugDetectionRange;
#endif

		public abstract void Launch();

		protected virtual void Start()
		{
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
			isDebugRange = true;
			this.debugDetectionRange = detectionRange;
			yield return new WaitForSeconds(duration);
			isDebugRange = false;
		}

		private void OnDrawGizmos()
		{
			if(isDebugRange)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(PlayerBehaviour.Instance.CenterPivot.position, new Vector3(debugDetectionRange, 1, 0));
			}
		}
#endif
	}
}