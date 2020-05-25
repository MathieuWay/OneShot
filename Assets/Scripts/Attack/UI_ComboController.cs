using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class UI_ComboController : MonoBehaviour
	{
		[SerializeField] private GameObject comboPanel = null;
		[SerializeField] private GameObject inputPrefab = null;
		[SerializeField] private GameObject comboPrefab = null;
		[SerializeField] private Transform comboContainer = null;
		[SerializeField] private Transform comboListContainer = null;
		private List<UI_Input> uiInputs;
		private bool isRunning;
		private bool nextInput;
		private bool displayCombo = false;
		private int currentInput;


		private void Awake()
		{
			comboPanel.SetActive(false);
		}

		private void Start()
		{
			isRunning = false;
			nextInput = false;
			currentInput = 0;
			uiInputs = new List<UI_Input>();

			ComboController.Instance.InitCombosEvent += InitCombos;
			ComboController.Instance.StartComboEvent += StartCombo;
			ComboController.Instance.ComboFailedEvent += ClearCombo;
			ComboController.Instance.ComboCanceledEvent += ClearCombo;
			ComboController.Instance.ComboCompletedEvent += ClearCombo;
			ComboController.Instance.NextInputEvent += delegate { nextInput = true; };
			ComboController.Instance.DisplayComboEvent += DisplayCombo;
		}

		private void InitCombos(Combo[] combos)
		{
			comboPanel.SetActive(true);

			for (int i = 0; i < combos.Length; i++)
			{
				GameObject instance = Instantiate(comboPrefab, comboListContainer);

				instance.GetComponent<UI_Combo>().Init(combos[i].attackTitle.ToString().ToUpper(), combos[i]);
			}
		}

		private void StartCombo(Combo combo)
		{
			if (LevelController.Instance.phase != Phase.Combat) return;

			if(displayCombo)
			{
				ClearCombo();
				displayCombo = false;
			}

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

		private void DisplayCombo(Combo combo)
		{
			if (LevelController.Instance.phase != Phase.Combat) return;

			if(displayCombo)
			{
				ClearCombo();
			}

			displayCombo = true;

			for (int i = 0; i < combo.inputs.Length; i++)
			{
				GameObject instance = Instantiate(inputPrefab, comboContainer);

				UI_Input uiInput = instance.GetComponent<UI_Input>();
				uiInput.Init(UI_Gamepad.Instance.GetInputSprite(combo.inputs[i].inputName));
				uiInputs.Add(uiInput);
			}
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