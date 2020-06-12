using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	[CreateAssetMenu(menuName = "OneShot/EnemyData")]
	public class EnemyData : ScriptableObject
	{
		[SerializeField] private EnemyName enemyName = EnemyName.Basic;
		[SerializeField] private float speed = 1;
		[SerializeField] private float reactionTime = 1;
		[SerializeField] private float fieldOfView = 1;
		public EnemyName _EnemyName { get => enemyName; }
		public float Speed { get => speed; }
		public float ReactionTime { get => reactionTime; }
		public float FieldOfView { get => fieldOfView; }
	}

	public enum EnemyName
	{
		Basic,
		Ninja,
		Shielded,
		Bulldozer,
	}
}