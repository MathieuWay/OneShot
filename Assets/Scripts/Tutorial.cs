using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
	public static Tutorial Instance { get; private set; }

	public delegate void TutoDelegate();
	public event TutoDelegate OnDestroyTuto;

	[SerializeField] private TextMeshProUGUI tutorialText = null;
	[SerializeField] private GameObject messageContainer = null;
	[SerializeField] private TutoStep[] tutoSteps = null;

	private bool stepCompleted = false;
	private bool tutorialCompleted = false;

	[System.Serializable]
	public class TutoStep
	{
		[SerializeField] private float messageDuration = 0;
		[SerializeField] [TextArea] private string message = string.Empty;
		[SerializeField] private StepName stepName = StepName.None;

		public float MessageDuration { get => messageDuration; }
		public string Message { get => message; }
		public StepName _StepName { get => stepName; }
	}

	public enum StepName
	{
		None,
		WaitPointPlacement,
		WaitPointDisplacement,
		WaitPointChangeTiming,
		WaitGamePause
	}

	private void Awake()
	{
		if(Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		DontDestroyOnLoad(gameObject);

		DisplayMessage(false);
	}

	private void RemoveTutorial()
	{
		Debug.Log("REMOVE TUTORIAL " + name);
		OnDestroyTuto?.Invoke();
		Destroy(gameObject);
		Instance = null;
	}

	private void Start()
	{
		LevelLoader.Instance.OnStartLoadNextLevel += RemoveTutorial;

		if (tutorialCompleted) return;

		StartCoroutine(TutorialProcess());
	}

	private IEnumerator TutorialProcess()
	{
		int currentStep = 0;
		oneShot.LevelController.Instance.LockCombatPhase = true;

		while(currentStep < tutoSteps.Length)
		{
			stepCompleted = false;

			tutorialText.text = tutoSteps[currentStep].Message;
			DisplayMessage(true);

			//Ajout des events liés aux actions du tutoriel
			switch(tutoSteps[currentStep]._StepName)
			{
				case StepName.None:
					StartCoroutine(MessageDuration(tutoSteps[currentStep].MessageDuration));
					yield return new WaitForSeconds(tutoSteps[currentStep].MessageDuration);
					stepCompleted = true;
					break;

				case StepName.WaitGamePause:
					UI_Timeline.Instance.OnSetPause += CompleteStep;
					break;

				case StepName.WaitPointChangeTiming:
					UI_PointController.Instance.OnPointMoved += CompleteStep;
					break;

				case StepName.WaitPointDisplacement:
					SpawnController.Instance.UngrabPointEvent += CompleteStep;
					break;

				case StepName.WaitPointPlacement:
					SpawnController.Instance.SpawnPointEvent += CompleteStep;
					break;
			}

			while (!stepCompleted)
			{
				yield return null;
			}

			//On retire les events
			switch (tutoSteps[currentStep]._StepName)
			{
				case StepName.WaitGamePause:
					UI_Timeline.Instance.OnSetPause -= CompleteStep;
					break;

				case StepName.WaitPointChangeTiming:
					UI_PointController.Instance.OnPointMoved -= CompleteStep;
					break;

				case StepName.WaitPointDisplacement:
					SpawnController.Instance.UngrabPointEvent -= CompleteStep;
					break;

				case StepName.WaitPointPlacement:
					SpawnController.Instance.SpawnPointEvent -= CompleteStep;
					break;
			}

			yield return null;
			currentStep++;
		}

		oneShot.LevelController.Instance.LockCombatPhase = false;
		tutorialCompleted = true;
	}

	private IEnumerator MessageDuration(float duration)
	{
		yield return new WaitForSeconds(duration);

		DisplayMessage(false);
	}

	private void CompleteStep()
	{
		stepCompleted = true;
	}
	private void CompleteStep(SpawnPoint spawnPoint)
	{
		stepCompleted = true;
	}

	private void DisplayMessage(bool display)
	{
		messageContainer.SetActive(display);

		if (!display)
		{
			tutorialText.text = string.Empty;
		}
	}
}