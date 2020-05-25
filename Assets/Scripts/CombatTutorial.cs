using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using oneShot;


public class CombatTutorial : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI messageText = null;
	[SerializeField] private GameObject messageContainer = null;
	[SerializeField] private TutoStep[] tutoSteps = null;
	[SerializeField] private EnemyStep[] enemySteps = null;
	private int currentStep = 0;
	private bool stepCompleted = false;

	[System.Serializable]
	public class TutoStep
	{
		[SerializeField] private float messageDuration = 0;
		[SerializeField] [TextArea] private string message = string.Empty;
		[SerializeField] private StepName stepName = StepName.None;
		public string Message { get => message; }
		public float MessageDuration { get => messageDuration; }
		public StepName _StepName { get => stepName; }
	}

	[System.Serializable]
	public class EnemyStep
	{
		[SerializeField] private EnemyName enemyName = EnemyName.Basic;
		[SerializeField] private AttackName attackName = AttackName.SlashAttack;
		[SerializeField] [TextArea] private string message = string.Empty;
		public EnemyName _EnemyName { get => enemyName; }
		public AttackName _AttackName { get => attackName; }
		public string Message { get => message; }
		public bool StepCompleted { get; set; }
		public bool EnemyKilled { get; set; }
	}

	public enum StepName
	{
		None,
		Attack,
	}

	private void Start()
	{
		SceneManager.sceneLoaded += ResetTuto;
		Tutorial.Instance.OnDestroyTuto += RemoveTuto;
		LevelController.Instance.OnLaunchCombatPhase += delegate { StartCoroutine(TutorialProcess()); };
		currentStep = 0;
		DisplayMessage(false);
	}

	private void RemoveTuto()
	{
		SceneManager.sceneLoaded -= ResetTuto;
	}

	private void ResetTuto(Scene scene, LoadSceneMode loadSceneMode)
	{
		LevelController.Instance.OnLaunchCombatPhase += delegate { StartCoroutine(TutorialProcess()); };
		currentStep = 0;
		StopAllCoroutines();
		DisplayMessage(false);
	}

	private IEnumerator TutorialProcess()
	{
		while(currentStep < tutoSteps.Length)
		{
			stepCompleted = false;

			switch(tutoSteps[currentStep]._StepName)
			{
				case StepName.None:
					messageText.text = tutoSteps[currentStep].Message;
					DisplayMessage(true);
					yield return new WaitForSeconds(tutoSteps[currentStep].MessageDuration);
					DisplayMessage(false);
					stepCompleted = true;
					break;

				case StepName.Attack:

					TacticsController.Instance.OnPlayerTeleport += CheckNearestEnemy;
					EnemyBase.OnEnemyDeadGetInfo += CheckAllEnemiesKilled;

					break;
			}

			while(!stepCompleted)
			{
				yield return null;
			}

			currentStep++;
		}
	}

	private void CheckNearestEnemy()
	{
		EnemyBase nearestEnemy = AttackController.Instance.FindNearestEnemy();

		for (int i = 0; i < enemySteps.Length; i++)
		{
			if (!enemySteps[i].StepCompleted && enemySteps[i]._EnemyName == nearestEnemy._EnemyData._EnemyName)
			{
				messageText.text = enemySteps[i].Message;
				DisplayMessage(true);
				StartCoroutine(EnemyMessageProcess(5, enemySteps[i]._AttackName));

				enemySteps[i].StepCompleted = true;
				break;
			}
		}
	}

	private void CheckAllEnemiesKilled(EnemyBase enemyBase)
	{
		int enemyKilledCount = 0;

		for (int i = 0; i < enemySteps.Length; i++)
		{
			//Vérification du nom de l'ennemi tué
			if(!enemySteps[i].EnemyKilled && enemySteps[i]._EnemyName == enemyBase._EnemyData._EnemyName)
			{
				//L'ennemi est bien tué
				enemySteps[i].EnemyKilled = true;
			}

			if(enemySteps[i].EnemyKilled) enemyKilledCount++;
		}

		//Tant qu'on atteint pas le nombre d'étape d'ennemi, le joueur ne les a pas tous tués
		if (enemyKilledCount < enemySteps.Length) return;

		stepCompleted = true;
	}

	private IEnumerator EnemyMessageProcess(float delay, AttackName attackName)
	{
		ComboController.Instance.LockCombo = true;

		yield return new WaitForEndOfFrame();

		//Attente d'une frame pour mettre le temps en pause
		GameTime.Instance.CancelCoroutine();
		GameTime.Instance.SetHardTimeSpeed(0);

		yield return new WaitForSeconds(delay);

		ComboController.Instance.LockCombo = false;
		DisplayMessage(false);
		GameTime.Instance.SetHardTimeSpeed(1);

		AttackController.Instance.DisplayAttack(attackName);
	}

	private void DisplayMessage(bool display)
	{
		messageContainer.SetActive(display);

		if (!display)
		{
			messageText.text = string.Empty;
		}
	}
}