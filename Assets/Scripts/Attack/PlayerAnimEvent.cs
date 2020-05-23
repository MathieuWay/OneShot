using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class PlayerAnimEvent : MonoBehaviour
	{
		public delegate void PlayerDelegate();
		public event PlayerDelegate OnAnimationEvent;

		public void OnKeyEvent()
		{
			OnAnimationEvent?.Invoke();
		}
	}
}