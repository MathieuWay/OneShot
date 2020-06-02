using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoader : MonoBehaviour
{
	public static LevelLoader Instance { get; private set; }

	public delegate void LevelDelegate();
	public event LevelDelegate OnStartLoadNextLevel;

	[SerializeField] private LevelName nextLevel = LevelName.None;
	[SerializeField] private LevelData[] levels = null;
	public LevelName NextLevel { get => nextLevel; }
	public LevelName CurrentLevel { get; private set; }
	private Dictionary<LevelName, string> levelsDic;

	[System.Serializable]
	public class LevelData
	{
		[SerializeField] private LevelName levelName = LevelName.None;
		[SerializeField] private string level = string.Empty;
		public string Level { get => level; }
		public LevelName LevelName { get => levelName; }
	}

	public enum LevelName
	{
		None,
		Tuto1,
		Tuto2,
		Tuto3,
		Tuto4,
		Level1,
		Level2
	}


	public void LoadNextLevel()
	{
		OnStartLoadNextLevel?.Invoke();
		Fader.Instance.FadeOut();
		StartCoroutine(LoadDelay(0.1f));
	}

	private IEnumerator LoadDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(levelsDic[nextLevel]);
	}

	private void Awake()
	{
		if(Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		levelsDic = new Dictionary<LevelName, string>();

		for (int i = 0; i < levels.Length; i++)
		{
			levelsDic.Add(levels[i].LevelName, levels[i].Level);

			if(levels[i].Level == SceneManager.GetActiveScene().name)
			{
				CurrentLevel = levels[i].LevelName;
			}
		}
	}
}