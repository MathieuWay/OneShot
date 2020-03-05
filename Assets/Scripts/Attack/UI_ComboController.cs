using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class UI_ComboController : MonoBehaviour
	{
		[SerializeField] private GameObject inputPrefab;
		[SerializeField] private Transform comboContainer;
		private List<UI_Input> uiInputs;
		private bool isRunning;
		private bool nextInput;
		private int currentInput;


		private void Start()
		{
			isRunning = false;
			nextInput = false;
			currentInput = 0;
			uiInputs = new List<UI_Input>();

			ComboController.Instance.StartComboEvent += StartCombo;
			ComboController.Instance.ComboFailedEvent += ClearCombo;
			ComboController.Instance.ComboCompletedEvent += ClearCombo;
			ComboController.Instance.NextInputEvent += delegate { nextInput = true; };
		}

		private void StartCombo(Combo combo)
		{
			if (LevelController.Instance.phase != Phase.Combat) return;

			for (int i = 0; i < combo.inputs.Length; i++)
			{
				GameObject instance = Instantiate(inputPrefab, comboContainer);

				UI_Input uiInput = instance.GetComponent<UI_Input>();
				uiInput.Init(UI_Gamepad.Instance.GetInputSprite(combo.inputs[i].inputName));
				uiInputs.Add(uiInput);
			}

			isRunning = true;
			nextInput = false;
			currentInput = 0;
			
			StartCoroutine(ComboProcess(combo));
		}

		private void ClearCombo()
		{
			if (LevelController.Instance.phase != Phase.Combat) return;

			isRunning = false;
			nextInput = false;
			currentInput = 0;

			for (int i = 0; i < uiInputs.Count; i++)
			{
				Destroy(uiInputs[i].gameObject);
			}

			uiInputs.Clear();
		}

		private IEnumerator ComboProcess(Combo combo)
		{
			float timer = combo.inputs[currentInput].delay;

			while(isRunning)
			{
				if(nextInput)
				{
					nextInput = false;

					uiInputs[currentInput].DisplayProgress(0);

					currentInput++;
					timer = combo.inputs[currentInput].delay;

					yield return null;
					continue;
				}

				if (timer <= 0) yield break;

				uiInputs[currentInput].DisplayProgress(timer / combo.inputs[currentInput].delay);

				timer -= Time.deltaTime;
				yield return null;
			}
		}
	}
}