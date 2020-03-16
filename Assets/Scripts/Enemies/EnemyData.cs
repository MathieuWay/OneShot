using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	[CreateAssetMenu(menuName = "OneShot/EnemyData")]
	public class EnemyData : ScriptableObject
	{
		[SerializeField] private float speed;
		[SerializeField] private float reactionTime;
		[SerializeField] private float fieldOfView;
		public float Speed { get => speed; }
		public float ReactionTime { get => reactionTime; }
		public float FieldOfView { get => fieldOfView; }
	}
}