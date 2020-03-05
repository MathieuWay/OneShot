﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class UI_Gamepad : MonoBehaviour
	{
		public static UI_Gamepad Instance { get; private set; }

		[System.Serializable]
		public class UI_Input
		{
			[SerializeField] private InputName inputName;
			[SerializeField] private Sprite inputSprite;
			public InputName InputName { get => inputName; }
			public Sprite InputSprite { get => inputSprite; }
		}

		[SerializeField] private UI_Input[] uiInputs;
		private Dictionary<InputName, UI_Input> uiInputDic;


		public Sprite GetInputSprite(InputName inputName)
		{
			return uiInputDic[inputName].InputSprite;
		}

		private void Awake()
		{
			if(Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;

			uiInputDic = new Dictionary<InputName, UI_Input>();

			for (int i = 0; i < uiInputs.Length; i++)
			{
				uiInputDic.Add(uiInputs[i].InputName, uiInputs[i]);
			}
		}
	}
}