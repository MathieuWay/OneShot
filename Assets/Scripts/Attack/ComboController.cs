using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public enum InputName { A, B, X, Y, Joy_Up, Joy_Down, Joy_Left, Joy_Right, Joy_Loop, Joy_QuarterLoop, Joy_HalfLoop, RT, LT }


	[System.Serializable]
	public class Combo
	{
		public ComboInput[] inputs;
		public float repeatDelay = 2;
		public string attackTitle;

		[System.Serializable]
		public class ComboInput
		{
			public InputName inputName;
			public float delay = 1;
		}

		public AttackName _AttackName { get; private set; }
		public bool IsStopped { get; set; }

		public void Init(AttackName attackName)
		{
			_AttackName = attackName;
		}
	}


	public class ComboController : MonoBehaviour
	{
		public static ComboController Instance { get; private set; }

		public delegate void ComboDelegate(AttackName comboName);
		public delegate void ComboDelegateV2(Combo combo);
		public delegate void ComboDelegateV3();
		public delegate void ComboDelegateV4(Combo[] combos);
		public event ComboDelegateV4 InitCombosEvent;
		public event ComboDelegate ComboSuccessEvent;
		public event ComboDelegate StartComboNameEvent;
		public event ComboDelegateV2 StartComboEvent;
		public event ComboDelegateV2 DisplayComboEvent;
		public event ComboDelegateV3 ComboFailedEvent;
		public event ComboDelegateV3 ComboCanceledEvent;
		public event ComboDelegateV3 ComboCompletedEvent;
		public event ComboDelegateV3 NextInputEvent;

		[SerializeField] private float startNewComboOnFailedDelay = 1;
		[SerializeField] private float startNewComboOnSucceedDelay = 0.5f;

		private bool comboRunning = false;
		private float joystickLimit = 0.8f;

		public bool LockCombo { get; set; }


		public void StartCombos(Combo[] combos)
		{
			StartCoroutine(ComboSelection(combos));

			InitCombosEvent?.Invoke(combos);
		}

		public void DisplayCombo(Combo combo)
		{
			if (LockCombo) return;

			DisplayComboEvent?.Invoke(combo);
		}

		public void CancelCombo()
		{
			ComboCanceledEvent?.Invoke();
			comboRunning = false;
		}


		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}


		private IEnumerator ComboSelection(Combo[] combos)
		{
			while (true)
			{
				if (comboRunning || LockCombo)
				{
					yield return null;
					continue;
				}

				yield return null;

				for (int i = 0; i < combos.Length; i++)
				{
					bool startCombo = CheckInput(combos[i].inputs[0].inputName);

					if (startCombo && !combos[i].IsStopped)
					{
						//Debug.Log("______________________\n START ATTACK: " + combos[i]._AttackName.ToString());

						StartComboEvent?.Invoke(combos[i]);
						StartComboNameEvent?.Invoke(combos[i]._AttackName);

						StartCoroutine(ComboProcess(combos[i]));
						break;
					}
				}
			}
		}

		private IEnumerator ComboProcess(Combo combo)
		{
			comboRunning = true;

			int comboID = 0;
			float timer = 0;

			while (comboID < combo.inputs.Length)
			{
				//Si on stop brutalement un combo (annulation), on stop la coroutine
				if(!comboRunning)
				{
					yield break;
				}

				//Vérification de la réussite de l'input
				inputSucceed = CheckInput(combo.inputs[comboID].inputName);

				if (inputSucceed)
				{
					//Debug.Log("SUCCEED: " + combo.inputs[comboID].ToString());
					inputSucceed = false;
					comboID++;
					NextInputEvent?.Invoke();

					if (comboID >= combo.inputs.Length)
					{
						//Debug.Log("COMBO COMPLETED");
						ComboSuccessEvent?.Invoke(combo._AttackName);
						ComboCompletedEvent?.Invoke();

						StartCoroutine(ComboReadyProcess(combo));

						yield return new WaitForSeconds(startNewComboOnSucceedDelay);

						comboRunning = false;
						yield break;
					}

					timer = 0;
					yield return null;
					continue;
				}

				//Vérification d'un mauvais input
				bool comboFailed = CheckBadInput(combo.inputs[comboID].inputName);

				//Mauvais input ou temps dépassé
				//Pour le mauvais input, on le compte après un délai 
				//pour éviter d'entrer en conflit avec l'input précédent
				if ((comboFailed && timer > 0.3f) || timer > combo.inputs[comboID].delay)
				{
					if (comboFailed)
					{
						//Debug.Log("FAILED: " + combo.inputs[comboID]);
					}
					else
					{
						//Debug.Log("FAILED: TIME OUT");
					}

					ComboFailedEvent?.Invoke();

					yield return new WaitForSeconds(startNewComboOnFailedDelay);

					comboRunning = false;
					yield break;
				}

				timer += Time.deltaTime;
				yield return null;
			}
		}

		private IEnumerator ComboReadyProcess(Combo combo)
		{
			combo.IsStopped = true;
			yield return new WaitForSeconds(combo.repeatDelay);
			combo.IsStopped = false;
		}

		private bool inputSucceed = false;

		private bool CheckInput(InputName inputName)
		{
			bool succeed = false;

			Gamepad gamepad = Gamepad.Instance;

			switch (inputName)
			{
				case InputName.A: succeed = gamepad.ButtonDownA; break;
				case InputName.B: succeed = gamepad.ButtonDownB; break;
				case InputName.X: succeed = gamepad.ButtonDownX; break;
				case InputName.Y: succeed = gamepad.ButtonDownY; break;
				case InputName.RT: succeed = gamepad.TriggerR; break;
				case InputName.LT: succeed = gamepad.TriggerL; break;
				case InputName.Joy_Left: succeed = gamepad.HorizontalJL < -joystickLimit || gamepad.PadHorizontal < -joystickLimit; break;
				case InputName.Joy_Right: succeed = gamepad.HorizontalJL > joystickLimit || gamepad.PadHorizontal > joystickLimit; break;
				case InputName.Joy_Up: succeed = gamepad.VerticalJL > joystickLimit || gamepad.PadVertical > joystickLimit; break;
				case InputName.Joy_Down: succeed = gamepad.VerticalJL < -joystickLimit || gamepad.PadVertical < -joystickLimit; break;
				case InputName.Joy_QuarterLoop: succeed = loopSucceed; CheckJoyLoop(0.25f, 1); break;
				case InputName.Joy_HalfLoop: succeed = loopSucceed; CheckJoyLoop(0.5f, 1); break;
				case InputName.Joy_Loop: succeed = loopSucceed; CheckJoyLoop(1, 1); break;
			}

			return succeed;
		}

		private bool CheckBadInput(InputName rightInput)
		{
			bool fail = false;

			//!Critical: pas de mauvais input quand on fait la boucle avec le joystick, sinon impossible de faire la boucle
			if (rightInput == InputName.Joy_QuarterLoop || rightInput == InputName.Joy_HalfLoop || rightInput == InputName.Joy_Loop)
			{
				return false;
			}

			Gamepad gamepad = Gamepad.Instance;

			//BUTTONS
			if ((gamepad.ButtonDownA && rightInput != InputName.A) || (gamepad.ButtonDownB && rightInput != InputName.B)
				|| (gamepad.ButtonDownX && rightInput != InputName.X) || (gamepad.ButtonDownY && rightInput != InputName.Y)
				|| (gamepad.TriggerR && rightInput != InputName.RT) || (gamepad.TriggerL && rightInput != InputName.LT))
			{
				fail = true;
			}
			//JOYSTICK HORIZONTAL
			else if (Mathf.Abs(gamepad.HorizontalJL) > joystickLimit)
			{
				if ((gamepad.HorizontalJL > joystickLimit && rightInput != InputName.Joy_Right) ||
					(gamepad.HorizontalJL < -joystickLimit && rightInput != InputName.Joy_Left))
				{
					fail = true;
				}
			}
			//JOYSTICK VERTICAL
			else if (Mathf.Abs(gamepad.VerticalJL) > joystickLimit)
			{
				if ((gamepad.VerticalJL > joystickLimit && rightInput != InputName.Joy_Up) ||
					 (gamepad.VerticalJL < -joystickLimit && rightInput != InputName.Joy_Down))
				{
					fail = true;
				}
			}
			//PAD HORIZONTAL
			else if(gamepad.PadHorizontal != 0)
			{
				if((gamepad.PadHorizontal > 0 && rightInput != InputName.Joy_Right) || 
					(gamepad.PadHorizontal < 0 && rightInput != InputName.Joy_Left))
				{
					fail = true;
				}
			}
			//PAD VERTICAL
			else if(gamepad.PadVertical != 0)
			{
				if ((gamepad.PadVertical > 0 && rightInput != InputName.Joy_Up) ||
					(gamepad.PadVertical < 0 && rightInput != InputName.Joy_Down))
				{
					fail = true;
				}
			}

			return fail;
		}


		private bool isCheckingJoyLoop;
		private bool loopSucceed;

		private void CheckJoyLoop(float percentage, float duration)
		{
			if (isCheckingJoyLoop) return;

			StartCoroutine(JoyLoopProcess(percentage, duration));
		}

		private IEnumerator JoyLoopProcess(float percentage, float duration)
		{
			isCheckingJoyLoop = true;

			float t = 0;
			Gamepad gamepad = Gamepad.Instance;
			float lastAngle = 0;
			float angle = 0;
			bool firstAngle = false;

			while(t < duration)
			{
				if(Mathf.Abs(gamepad.VerticalJL) > 0.7f || Mathf.Abs(gamepad.HorizontalJL) > 0.7f)
				{
					if(!firstAngle)
					{
						firstAngle = true;
						lastAngle = Mathf.Atan2(gamepad.VerticalJL, gamepad.HorizontalJL) * Mathf.Rad2Deg;
					}

					float newAngle = Mathf.Atan2(gamepad.VerticalJL, gamepad.HorizontalJL) * Mathf.Rad2Deg;
					angle += Mathf.Abs(Mathf.DeltaAngle(lastAngle, newAngle));
					lastAngle = newAngle;
				}

				//360 pour un tour - 30 (arbitraire) pour un peu plus de souplesse
				if(angle >= (percentage * 360) - 30)
				{
					loopSucceed = true;
					yield return new WaitForEndOfFrame();
					loopSucceed = false;
				}

				t += Time.deltaTime;
				yield return null;
			}

			isCheckingJoyLoop = false;
		}
	}
}