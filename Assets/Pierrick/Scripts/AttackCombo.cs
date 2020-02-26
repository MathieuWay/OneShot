using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AttackCombo : MonoBehaviour
{
	[SerializeField] private Combo[] combos;

	private void Start()
	{
		ComboController.Instance.StartCombos(combos);
	}
}