using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Astra
{
	public class MusicManager : MonoBehaviour
	{
		[SerializeField] private AudioClip[] musics = null;
		[SerializeField] private AudioSource source = null;

		private void Start()
		{
			int randomMusic = Random.Range(0, musics.Length);

			source.clip = musics[randomMusic];
			source.Play();
		}
	}
}