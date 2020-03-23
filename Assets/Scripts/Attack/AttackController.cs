using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public enum AttackName { SpiralAttack, ThrustAttack, SlashAttack }


	public class AttackController : MonoBehaviour
	{
		public static AttackController Instance { get; private set; }

		[SerializeField] private Attack[] attacks;
		private Dictionary<AttackName, Attack> attacksDic;

		public void LaunchAttack(AttackName attackName)
		{
			if (LevelController.Instance.phase != Phase.Combat) return;

			attacksDic[attackName].Launch();
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

			ComboController.Instance.StartCombos(combos.ToArray());
			ComboController.Instance.ComboSuccessEvent += LaunchAttack;
		}
	}
}