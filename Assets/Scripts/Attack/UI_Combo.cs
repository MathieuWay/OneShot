using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

namespace oneShot
{
	public class UI_Combo : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI comboText = null;
		[SerializeField] private GameObject inputPrefab = null;
		[SerializeField] private Transform inputContainer = null;

		public void Init(string title, Combo combo)
		{
			comboText.text = title;

			for (int i = 0; i < combo.inputs.Length; i++)
			{
				GameObject instance = Instantiate(inputPrefab, inputContainer);

				UI_Input uiInput = instance.GetComponent<UI_Input>();
				uiInput.Init(UI_Gamepad.Instance.GetInputSprite(combo.inputs[i].inputName));
			}
		}
	}
}