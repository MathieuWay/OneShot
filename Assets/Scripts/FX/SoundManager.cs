using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class SoundManager : MonoBehaviour
	{
		public static SoundManager Instance { get; private set; }

		[SerializeField] private AudioData[] clips = null;
		[SerializeField] private AudioSource mainSource = null;

		[System.Serializable]
		public class AudioData
		{
			[SerializeField] private AudioClip clip = null;
			[SerializeField] private string clipName = string.Empty;
			public AudioClip Clip { get => clip; }
			public string ClipName { get => clipName; }
		}

		public void PlaySound(string soundName)
		{
			if(mainSource.isPlaying)
			{
				//Si la source est déjà entrain de jouer un son, on lance un son dans l'espace 3D
				PlaySoundAutomatically(soundName);
				return;
			}

			mainSource.clip = GetAudioClip(soundName);
			mainSource.Play();
		}
		public void PlaySoundAtPosition(string soundName, Vector3 position)
		{
			//Créé une audio source pour jouer le son à une position définie
			AudioSource.PlayClipAtPoint(GetAudioClip(soundName), position);
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
		private void PlaySoundAutomatically(string soundName)
		{
			//Position par défaut : Vector3.zero
			PlaySoundAtPosition(soundName, Vector3.zero);
		}
		private AudioClip GetAudioClip(string clipName)
		{
			for (int i = 0; i < clips.Length; i++)
			{
				if(clips[i].ClipName == clipName)
				{
					return clips[i].Clip;
				}
			}

			Debug.LogError("ERROR: AUDIO CLIP NOT FOUND");
			return null;
		}
	}
}