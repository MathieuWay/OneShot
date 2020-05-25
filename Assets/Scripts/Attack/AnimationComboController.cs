using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class AnimationComboController : MonoBehaviour
	{
		[SerializeField] private ComboController comboController;
		private Animator anim;
		private bool comboSucceed;
		private int comboStep;
		private int stopAnimCount;

		private void Start()
		{
			anim = PlayerBehaviour.Instance.ChronaAnim;
			PlayerBehaviour.Instance.PlayerAnimEvent.OnAnimationEvent += StopAnim;
			comboController.NextInputEvent += PlayAnim;
			comboController.StartComboEvent += delegate { ResetInfo(); };
			comboController.ComboFailedEvent += delegate { ResetAnim(); };
			comboController.ComboCanceledEvent += delegate { ResetAnim(); };
		}

		private void StopAnim()
		{
			if (comboSucceed) return;

			stopAnimCount++;

			if (comboStep >= stopAnimCount) return;

			anim.speed = 0;
		}

		private void PlayAnim()
		{
			comboStep++;
			anim.speed = 1;
		}

		private void ResetInfo()
		{
			comboSucceed = false;
			comboStep = 0;
			stopAnimCount = 1;
			anim.speed = 1;
		}

		private void ResetAnim()
		{
			anim.Rebind();
			ResetInfo();
		}
	}
}