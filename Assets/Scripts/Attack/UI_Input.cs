using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace oneShot
{
	public class UI_Input : MonoBehaviour
	{
		[SerializeField] private Image inputImage;
		[SerializeField] private Image progressImage;

		public void Init(Sprite sprite)
		{
			inputImage.sprite = sprite;
			progressImage.fillAmount = 0;
		}

		public void DisplayProgress(float percentage)
		{
			progressImage.fillAmount = percentage;
		}
	}
}