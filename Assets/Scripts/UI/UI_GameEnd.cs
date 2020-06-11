using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_GameEnd : MonoBehaviour
{
	[SerializeField] private GameObject endPanel = null;
	[SerializeField] private GameObject victoryPanel = null;
	[SerializeField] private GameObject defeatPanel = null;
	[SerializeField] private GameObject buttonsContainer = null;
	[SerializeField] private TextMeshProUGUI resultText = null;
	[SerializeField] private Button retryButton = null;
	[SerializeField] private Button nextLevelButton = null;
	[SerializeField] private TextMeshProUGUI retryText = null;
	[SerializeField] private TextMeshProUGUI nextLevelText = null;
	private bool victory, defeat;
	private bool readyToReload;
	private enum DefeatType { PlayerDie, TimeElapsed }
	private float combatDuration = 0;
	private bool playCombatPhase = false;


	private void Awake()
	{
		endPanel.SetActive(false);
		victoryPanel.SetActive(false);
		defeatPanel.SetActive(false);
		buttonsContainer.SetActive(false);
	}

	private void Start()
	{
		oneShot.LevelController.Instance.OnStartCombatPhase += delegate { playCombatPhase = true; };
		oneShot.EnemiesController.Instance.OnAllEnemiesKilled += Victory;
		oneShot.LevelController.Instance.OnPlayerDie += delegate { Defeat(DefeatType.PlayerDie); };
		oneShot.LevelController.Instance.OnTimeElapsed += delegate { Defeat(DefeatType.TimeElapsed); };
		retryButton.onClick.AddListener(ReloadGame);
		nextLevelButton.onClick.AddListener(delegate { LevelLoader.Instance.LoadNextLevel(); });

		//if (LevelLoader.Instance.CurrentLevel == LevelLoader.LevelName.Level1) Victory();
	}

	private void Update()
	{
		//if(Gamepad.Instance.ButtonDownA)
		//{
		//	retryButton.Select();
		//}

		if(playCombatPhase)
		{
			combatDuration += Time.unscaledDeltaTime;
		}
	}

	private void ReloadGame()
	{
		if (!readyToReload) return;

		readyToReload = false;
		oneShot.LevelController.Instance.ReloadScene(1.5f);
	}

	private void Victory()
	{
		if (defeat || victory) return;

		victory = true;
		playCombatPhase = false;

		StartCoroutine(VictoryProcess());
	}
	private IEnumerator VictoryProcess()
	{
		yield return new WaitForSeconds(1);

		endPanel.SetActive(true);
		victoryPanel.SetActive(true);

		resultText.text = "YOU KILLED " + oneShot.EnemiesController.Instance.EnemyKilledCount
			+ " ENEMIES IN " + combatDuration.ToString("0.000")/*UI_Timeline.Instance.GetCurrentTime().ToString("0.00")*/ + " SECONDS";

		yield return new WaitForSeconds(1);

		buttonsContainer.SetActive(true);

		if(LevelLoader.Instance.NextLevel == LevelLoader.LevelName.None)
		{
			nextLevelButton.gameObject.SetActive(false);
			EventSystem.current.SetSelectedGameObject(retryButton.gameObject);
		}
		else
		{
			nextLevelButton.gameObject.SetActive(true);
			EventSystem.current.SetSelectedGameObject(nextLevelButton.gameObject);
		}

		retryText.text = "PLAY AGAIN";
		nextLevelText.text = "NEXT LEVEL";
		readyToReload = true;
	}

	private void Defeat(DefeatType defeatType)
	{
		if (victory || defeat) return;

		defeat = true;

		StartCoroutine(DefeatProcess(defeatType));
	}
	private IEnumerator DefeatProcess(DefeatType defeatType)
	{
		yield return new WaitForSeconds(1);

		endPanel.SetActive(true);
		defeatPanel.SetActive(true);
		resultText.enabled = false;
		oneShot.LevelController.Instance.ReloadScene(1.5f);

		//int enemyLeft = oneShot.EnemiesController.Instance.EnemyLeft;
		//int enemyKilled = oneShot.EnemiesController.Instance.EnemyKilledCount;

		//switch (defeatType)
		//{
		//	case DefeatType.PlayerDie:
		//		resultText.text = "YOU KILLED " + enemyKilled + " ENEMIES BUT DIED";
		//		break;

		//	case DefeatType.TimeElapsed:
		//		if(enemyKilled <= 0)
		//		{
		//			resultText.text = "YOU KILLED NO ENEMY";
		//		}
		//		else
		//		{
		//			resultText.text = "YOU KILLED " + enemyKilled.ToString() + (enemyKilled > 1 ? " ENEMIES" : " ENEMY") 
		//				+ " AND MISSED " + enemyLeft + (enemyLeft > 1 ? " OTHERS" : " OTHER");
		//		}
		//		break;
		//}

		//yield return new WaitForSeconds(1);

		//continueButton.gameObject.SetActive(true);
		//continueText.text = "RETRY";
		//readyToReload = true;
	}
}