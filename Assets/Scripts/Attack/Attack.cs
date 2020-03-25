using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public abstract class Attack : MonoBehaviour
	{
		[Header("MAIN")]
		[SerializeField] private AttackName attackName = AttackName.SpiralAttack;
		[SerializeField] private Combo combo = null;
		public AttackName _AttackName { get => attackName; }
		public Combo Combo { get => combo; }

#if UNITY_EDITOR
		private bool isDebugRange;
		private Vector2 debugDetectionVolume;
#endif

		public abstract void Launch();

		protected virtual void Start()
		{
		}

		protected Enemy GetNearestEnemy(Vector2 boxVolume)
		{
			Collider2D[] colliders = Physics2D.OverlapBoxAll(PlayerBehaviour.Instance.CenterPivot.position, new Vector2(boxVolume.x, boxVolume.y), 0);

#if UNITY_EDITOR
			StartCoroutine(DebugOverlapBox(1, boxVolume));
#endif

			List<Transform> enemies = new List<Transform>();

			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].CompareTag("Enemy"))
				{
					if(colliders[i].GetComponent<Enemy>().isAlive)
					{
						enemies.Add(colliders[i].transform);
					}
				}
			}

			float minDistance = Mathf.Infinity;
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
		private IEnumerator DebugOverlapBox(float duration, Vector2 volume)
		{
			isDebugRange = true;
			debugDetectionVolume = volume;
			yield return new WaitForSeconds(duration);
			isDebugRange = false;
		}

		private void OnDrawGizmos()
		{
			if(isDebugRange)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(PlayerBehaviour.Instance.CenterPivot.position, new Vector3(debugDetectionVolume.x, debugDetectionVolume.y, 0));
			}
		}
#endif
	}
}