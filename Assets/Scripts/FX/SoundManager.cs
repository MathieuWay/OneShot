using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace oneShot
{
	public class SoundManager : MonoBehaviour
	{
		private static SoundManager instance;
		public static SoundManager Instance
		{
			get
			{
				if (instance == null)
				{
					Debug.LogWarning("Warning: SoundManager not found");
					//TODO: instancier le prefab
					return null;
				}

				return instance;
			}
			private set { instance = value; }
		}

		[SerializeField] private AudioSource mainSource = null;
		[SerializeField] private AudioData[] clips = null;

		[System.Serializable]
		public class AudioData
		{
			[SerializeField] private string clipName = string.Empty;
			[SerializeField] private AudioClip clip = null;
			public AudioClip Clip { get => clip; }
			public string ClipName { get => clipName; }
		}

		public void PlaySound(string soundName, float volume = 1)
		{
			//PlayOneShot permet de lancer plusieurs sons en simultané
			mainSource.PlayOneShot(GetAudioClip(soundName), volume);
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