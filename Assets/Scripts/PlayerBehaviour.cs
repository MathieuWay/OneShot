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
			VFX_Manager.Instance.PlayVFX("chrona_death", CenterPivot.position - new Vector3(0, 0.2f, 0));

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
					SetAttackAnimDirection(attackName);
					break;

				case AttackName.ThrustAttack:
					chronaAnim.SetTrigger("Thrust");
					SetAttackAnimDirection(attackName);
					break;
			}
		}

		private void SetAttackAnimDirection(AttackName attackName)
		{
			EnemyBase nearestEnemy = AttackController.Instance.FindNearestEnemy();
			Transform target = nearestEnemy.transform;

			float dirValue = 1;

			bool right = true;

			if (target.position.x > CenterPivot.position.x)
			{
				right = true;
			}
			else if (target.position.x < CenterPivot.position.x)
			{
				right = false;
			}

			switch (attackName)
			{
				case AttackName.SpiralAttack:
					break;

				case AttackName.SlashAttack:
					dirValue = right ? 1 : -1;
					break;

				case AttackName.ThrustAttack:
					dirValue = right ? 1 : -1;
					break;
			}

			pivot.transform.localScale = new Vector3(dirValue, pivot.transform.localScale.y, pivot.transform.localScale.z);
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