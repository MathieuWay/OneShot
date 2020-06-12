using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class MusicManager : MonoBehaviour
	{
		public static MusicManager Instance { get; private set; }
		[SerializeField] private MusicData[] musics = null;
		[SerializeField] private AudioSource source = null;
		private MusicData currentMusic = null;
		private bool isInTransition = false;

		[System.Serializable]
		public class MusicData
		{
			[SerializeField] private string musicName = string.Empty;
			[SerializeField] private AudioClip musicClip = null;
			[SerializeField] private float volume = 1;
			public string MusicName { get => musicName; }
			public AudioClip MusicClip { get => musicClip; }
			public float Volume { get => volume; }
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
			PlayMusic("planning_phase");
			LevelController.Instance.OnLaunchCombatPhase += delegate { PlayMusic("combat_phase"); };

			//OLD
			//int randomMusic = Random.Range(0, musics.Length);
			//source.clip = musics[randomMusic];
			//source.Play();
		}

		public void PlayMusic(string musicName)
		{
			if (source.isPlaying)
			{
				if (isInTransition) return;

				//Transition
				MusicData nextMusic = GetMusicData(musicName);
				StartCoroutine(MusicTransitionProcess(nextMusic, 2));
				return;
			}
			
			//Lancement musique
			currentMusic = GetMusicData(musicName);
			source.clip = currentMusic.MusicClip;
			source.Play();
		}

		private MusicData GetMusicData(string musicName)
		{
			for (int i = 0; i < musics.Length; i++)
			{
				if(musics[i].MusicName == musicName)
				{
					return musics[i];
				}
			}

			Debug.LogError("MUSIC NOT FOUND");
			return null;
		}

		private IEnumerator FadeMusic(MusicData music, float duration = 1, bool fadeOut = true)
		{
			float speed = 0;
			float time = 0;

			if(fadeOut)
			{
				time = source.volume;
				speed = source.volume / duration; //Volume actuel à diminuer

				while (time > 0)
				{
					time -= Time.deltaTime * speed;
					source.volume = time;
					yield return null;
				}

				source.volume = 0;
			}
			else
			{
				time = 0;
				speed = music.Volume / duration; //Volume à atteindre

				while (time < music.Volume)
				{
					time += Time.deltaTime * speed;
					source.volume = time;
					yield return null;
				}

				Debug.Log("End: " + time + " " + music.Volume + " " + source.clip);

				source.volume = music.Volume;
			}
		}
		private IEnumerator MusicTransitionProcess(MusicData nextMusic, float duration = 1)
		{
			float fadeDuration = duration / 2;
			isInTransition = true;
			StartCoroutine(FadeMusic(currentMusic, fadeDuration));

			yield return new WaitForSeconds(fadeDuration);

			currentMusic = nextMusic;
			source.clip = currentMusic.MusicClip;
			source.Play();
			StartCoroutine(FadeMusic(currentMusic, fadeDuration, false));

			yield return new WaitForSeconds(fadeDuration);

			isInTransition = false;
		}
	}
}