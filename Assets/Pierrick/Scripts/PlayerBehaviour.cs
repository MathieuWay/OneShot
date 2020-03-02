using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class PlayerBehaviour : MonoBehaviour
	{
		public static PlayerBehaviour Instance { get; private set; }

		[SerializeField] private Transform centerPivot;
		public Transform CenterPivot { get => centerPivot; }

		private void Awake()
		{
			if(Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}
	}
}