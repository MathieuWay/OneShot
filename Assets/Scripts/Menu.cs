using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class Menu : MonoBehaviour
	{
		private void Update()
		{
			if(Gamepad.Instance.ButtonDownStart)
			{
				LevelLoader.Instance.LoadNextLevel();
				SoundManager.Instance.PlaySound("start_carnage");
			}
		}
	}
}