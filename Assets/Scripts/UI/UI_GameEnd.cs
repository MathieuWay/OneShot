using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_GameEnd : MonoBehaviour
{
	[SerializeField] private GameObject endPanel = null;
	[SerializeField] private GameObject victoryPanel = null;
	[SerializeField] private GameObject defeatPanel = null;
	[SerializeField] private TextMeshProUGUI resultText = null;
	[SerializeField] private Button continueButton = null;
	[SerializeField] private TextMeshProUGUI continueText = null;
	private bool victory, defeat;
	private bool readyToReload;
	private enum DefeatType { PlayerDie, TimeElapsed }


	private void Awake()
	{
		endPanel.SetActive(false);
		victoryPanel.SetActive(false);
		defeatPanel.SetActive(false);
		continueButton.gameObject.SetActive(false);
	}

	private void Start()
	{
		oneShot.EnemiesController.OnAllEnemiesKilled += Victory;
		oneShot.LevelController.Instance.OnPlayerDie += delegate { Defeat(DefeatType.PlayerDie); };
		oneShot.LevelController.Instance.OnTimeElapsed += delegate { Defeat(DefeatType.TimeElapsed); };
		continueButton.onClick.AddListener(ReloadGame);
	}

	private void Update()
	{
		if(Gamepad.Instance.ButtonDownA)
		{
			continueButton.Select();
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

		StartCoroutine(VictoryProcess());
	}
	private IEnumerator VictoryProcess()
	{
		yield return new WaitForSeconds(1);

		endPanel.SetActive(true);
		victoryPanel.SetActive(true);

		resultText.text = "YOU KILLED " + oneShot.EnemiesController.Instance.EnemyKilledCount
			+ " ENEMIES IN " + UI_Timeline.Instance.GetCurrentTime().ToString("0.00") + " SECONDS";

		yield return new WaitForSeconds(1);

		continueButton.gameObject.SetActive(true);
		continueText.text = "PLAY AGAIN";
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