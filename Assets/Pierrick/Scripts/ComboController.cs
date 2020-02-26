using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public enum InputName { A, B, X, Y, Joy_Up, Joy_Down, Joy_Left, Joy_Right }
public enum ComboName { SpiralAttack, ThrustAttack, SlashAttack }


[System.Serializable]
public class Combo
{
	public ComboName comboName;
	public InputName[] inputs;
	public float delay = 1f;
}


public class ComboController : MonoBehaviour
{
	public static ComboController Instance { get; private set; }

	private bool comboRunning = false;
	private float joystickLimit = 0.8f;

	public void StartCombos(Combo[] combos)
	{
		StartCoroutine(ComboSelection(combos));
	}


	private void Awake()
	{
		if(Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}


	private IEnumerator ComboSelection(Combo[] combos)
	{
		while(true)
		{
			if(comboRunning)
			{
				yield return null;
				continue;
			}

			yield return null;

			for (int i = 0; i < combos.Length; i++)
			{
				bool startCombo = CheckInput(combos[i].inputs[0]);

				if(startCombo)
				{
					Debug.Log("______________________\n START COMBO: " + combos[i].comboName.ToString());
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

		while(comboID < combo.inputs.Length)
		{
			//Vérification de la réussite de l'input
			bool comboSucceed = CheckInput(combo.inputs[comboID]);

			if(comboSucceed)
			{
				Debug.Log("SUCCEED: " + combo.inputs[comboID].ToString());
				comboID++;
				
				if (comboID >= combo.inputs.Length)
				{
					Debug.Log("COMBO COMPLETED");
					comboRunning = false;
					yield break;
				}

				timer = 0;
				yield return null;
				continue;
			}

			//Vérification d'un mauvais input
			bool comboFailed = CheckBadInput(combo.inputs[comboID]);

			//Mauvais input ou temps dépassé
			//Pour le mauvais input, on le compte après un délai 
			//pour éviter d'entrer en conflit avec l'input précédent
			if ((comboFailed && timer > 0.3f) || timer > combo.delay)
			{
				if(comboFailed)
				{
					Debug.Log("FAILED: " + combo.inputs[comboID]);
				}
				else
				{
					Debug.Log("FAILED: TIME OUT");
				}
			
				comboRunning = false;
				yield break;
			}

			timer += Time.deltaTime;
			yield return null;
		}
	}

	private bool CheckInput(InputName inputName)
	{
		bool succeed = false;

		Gamepad gamepad = Gamepad.Instance;

		switch(inputName)
		{
			case InputName.A: succeed = gamepad.ButtonDownA; break;
			case InputName.B: succeed = gamepad.ButtonDownB; break;
			case InputName.X: succeed = gamepad.ButtonDownX; break;
			case InputName.Y: succeed = gamepad.ButtonDownY; break;
			case InputName.Joy_Left: succeed = gamepad.HorizontalJL < -joystickLimit; break;
			case InputName.Joy_Right: succeed = gamepad.HorizontalJL > joystickLimit; break;
			case InputName.Joy_Up: succeed = gamepad.VerticalJL > joystickLimit; break;
			case InputName.Joy_Down: succeed = gamepad.VerticalJL < -joystickLimit; break;
		}

		return succeed;
	}

	private bool CheckBadInput(InputName rightInput)
	{
		bool fail = false;

		Gamepad gamepad = Gamepad.Instance;

		if((gamepad.ButtonDownA && rightInput != InputName.A) || (gamepad.ButtonDownB && rightInput != InputName.B)
			|| (gamepad.ButtonDownX && rightInput != InputName.X) || (gamepad.ButtonDownY && rightInput != InputName.Y))
		{
			fail = true;
		}
		else if(Mathf.Abs(gamepad.HorizontalJL) > joystickLimit)
		{
			if((gamepad.HorizontalJL > joystickLimit && rightInput != InputName.Joy_Right) ||
				(gamepad.HorizontalJL < -joystickLimit && rightInput != InputName.Joy_Left))
			{
				fail = true;
			}
		}
		else if(Mathf.Abs(gamepad.VerticalJL) > joystickLimit)
		{
			if ((gamepad.VerticalJL > joystickLimit && rightInput != InputName.Joy_Up) ||
				 (gamepad.VerticalJL < -joystickLimit && rightInput != InputName.Joy_Down))
			{
				fail = true;
			}
		}

		return fail;
	}
}