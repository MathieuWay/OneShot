using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public class PlayerBehaviour : MonoBehaviour
	{
		public static PlayerBehaviour Instance { get; private set; }

		[SerializeField] private Transform centerPivot = null;
		[SerializeField] private GameObject pivot = null;
		[SerializeField] private Animator chronaAnim = null;
		[SerializeField] private PlayerAnimEvent playerAnimEvent = null;
		public Transform CenterPivot { get => centerPivot; }
		public bool IsDead { get; private set; }
		public PlayerAnimEvent PlayerAnimEvent { get => playerAnimEvent; }
		public Animator ChronaAnim { get => chronaAnim; }

		private Animator anim;

		public void Kill()
		{
			if (IsDead) return;

			IsDead = true;

			//!TMP
			pivot.SetActive(false);

			anim.Play("dying");
			SoundManager.Instance.PlaySound("chrona_death");

			LevelController.Instance.PlayerDie();
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

		private void Start()
		{
			anim = GetComponentInChildren<Animator>();

			ComboController.Instance.StartComboNameEvent += PlayAttackAnim;

			TacticsController.Instance.OnBeforePlayerTeleport += PlayTpAnim;
		}

		private void PlayAttackAnim(AttackName attackName)
		{
			switch(attackName)
			{
				case AttackName.SpiralAttack:
					chronaAnim.SetTrigger("Spiral");
					break;

				case AttackName.SlashAttack:
					chronaAnim.SetTrigger("Slash");
					break;

				case AttackName.ThrustAttack:
					chronaAnim.SetTrigger("Thrust");
					break;
			}
		}

		private void PlayTpAnim()
		{
			StartCoroutine(WaitTpAnim());
		}
		private IEnumerator WaitTpAnim()
		{
			//Attente d'une frame pour attendre le reset de l'animator (avec AnimationComboController)
			yield return new WaitForEndOfFrame();
			chronaAnim.SetTrigger("TP");
		}
	}
}