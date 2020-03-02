using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public class AttackCombo : MonoBehaviour
	{
		[SerializeField] private Combo[] combos;

		private void Start()
		{
			ComboController.Instance.StartCombos(combos);
			ComboController.Instance.ComboSuccessEvent += AttackController.Instance.LaunchAttack;
		}
	}
}