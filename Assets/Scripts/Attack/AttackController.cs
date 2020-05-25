using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public enum AttackName { SpiralAttack, ThrustAttack, SlashAttack }


	public class AttackController : MonoBehaviour
	{
		public static AttackController Instance { get; private set; }

		[SerializeField] private Attack[] attacks = null;
		private Dictionary<AttackName, Attack> attacksDic;

		public void LaunchAttack(AttackName attackName)
		{
			if (LevelController.Instance.phase != Phase.Combat) return;

			attacksDic[attackName].Launch();
		}

		public void DisplayAttack(AttackName attackName)
		{
			if (LevelController.Instance.phase != Phase.Combat) return;

			ComboController.Instance.DisplayCombo(attacksDic[attackName].Combo);
		}

		public EnemyBase FindNearestEnemy()
		{
			float detectionRadius = 5;
			Vector2 playerPos = PlayerBehaviour.Instance.CenterPivot.position;
			Collider2D[] colliders = Physics2D.OverlapCircleAll(playerPos, detectionRadius);
			List<EnemyBase> enemies = new List<EnemyBase>();

			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].GetComponent<EnemyBase>() != null)
				{
					enemies.Add(colliders[i].GetComponent<EnemyBase>());
				}
			}

			float minDistance = detectionRadius;
			EnemyBase nearestEnemy = null;

			for (int i = 0; i < enemies.Count; i++)
			{
				float distance = Vector2.Distance(enemies[i].transform.position, playerPos);

				if (distance <= minDistance)
				{
					minDistance = distance;
					nearestEnemy = enemies[i];
				}
			}

			return nearestEnemy;
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
			attacksDic = new Dictionary<AttackName, Attack>();
			List<Combo> combos = new List<Combo>();

			for (int i = 0; i < attacks.Length; i++)
			{
				attacksDic.Add(attacks[i]._AttackName, attacks[i]);

				attacks[i].Combo.Init(attacks[i]._AttackName);
				combos.Add(attacks[i].Combo);
			}

			ComboController.Instance.ComboSuccessEvent += LaunchAttack;

			LevelController.Instance.OnStartCombatPhase += delegate { StartCoroutine(StartDelay(combos.ToArray())); };

			TacticsController.Instance.OnBeforePlayerTeleport += AttackOnBeforeTeleport;
			TacticsController.Instance.OnPlayerTeleport += AttackOnTeleport;
		}

		private IEnumerator StartDelay(Combo[] combos)
		{
			yield return new WaitForEndOfFrame();
			ComboController.Instance.StartCombos(combos);
		}

		private void AttackOnBeforeTeleport()
		{
			ComboController.Instance.LockCombo = true;
			ComboController.Instance.CancelCombo();
		}
		private void AttackOnTeleport()
		{
			ComboController.Instance.LockCombo = false;
			StartCoroutine(DisplayAttackOnTeleportDelay());
		}
		private IEnumerator DisplayAttackOnTeleportDelay()
		{
			yield return new WaitForEndOfFrame();

			EnemyBase nearestEnemy = FindNearestEnemy();
			AttackName attackToDisplay = AttackName.SpiralAttack;

			switch (nearestEnemy._EnemyData._EnemyName)
			{
				case EnemyName.Basic: attackToDisplay = AttackName.SpiralAttack; break;
				case EnemyName.Bulldozer: attackToDisplay = AttackName.ThrustAttack; break;
				case EnemyName.Ninja: attackToDisplay = AttackName.SlashAttack; break;
				case EnemyName.Shielded: attackToDisplay = AttackName.SpiralAttack; break;
			}

			DisplayAttack(attackToDisplay);
		}
	}
}