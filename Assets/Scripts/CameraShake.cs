using System.Collections;
using UnityEngine;


namespace oneShot
{
	public class CameraShake : MonoBehaviour
	{
        public class ShakeSetting
        {
            public float amount;
            public float duration;

            public ShakeSetting(float amount, float duration)
            {
                this.amount = amount;
                this.duration = duration;   
            }
        }
        public static class ShakeTemplate
        {
            public static readonly ShakeSetting defaultSetting = new ShakeSetting(2, 0.5f);
            public static readonly ShakeSetting inputValid = new ShakeSetting(1, 0.33f);
        }
		public static CameraShake Instance { get; private set; }

		public bool debugMode = false;//Test-run/Call ShakeCamera() on start

		[SerializeField] private float shakeAmount = 1;//The amount to shake this frame.
		[SerializeField] private float shakeDuration = 1;//The duration this frame.
		[SerializeField] private bool smooth = false;//Smooth rotation?
		[SerializeField] private float smoothAmount = 5f;//Amount to smooth
        public AnimationCurve shakeCurveAmount;

        private float initShakeAmount; //The initial shake amount (to determine percentage), set when ShakeCamera is called.
		private float initShakeDuration; //The initial shake duration, set when ShakeCamera is called.
		private float shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
		private bool isRunning = false; //Is the coroutine running right now?

		public bool IsActive { get; set; }


		private void Awake()
		{
			if (Instance != null)
			{
				enabled = false;
				return;
			}

			Instance = this;

			IsActive = true;
			initShakeAmount = shakeAmount;
			initShakeDuration = shakeDuration;
		}

		private void Start()
		{
			if (debugMode) ShakeCamera();
		}

        public void ShakeCamera(ShakeSetting shakeSetting)
        {
            ShakeCamera(shakeSetting.amount, shakeSetting.duration);
        }


        public void ShakeCamera(float amount = -1, float duration = -1)
		{
			if (!IsActive) return;

			//Reset shakeAmount + shakeDuration
			shakeAmount = amount != -1 ? amount : initShakeAmount;
			shakeDuration = duration != -1 ? duration : initShakeDuration;

			//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
			if (!isRunning) StartCoroutine(Shake());
		}


		IEnumerator Shake()
		{
			isRunning = true;

			Vector3 initEulerAngles = transform.eulerAngles;

            while (shakeDuration > 0.01f)
			{
                //A Vector3 to add to the Local Rotation
                Vector3 rotationAmount = initEulerAngles + Random.insideUnitSphere * shakeAmount;
				//Don't change the Z; it looks funny.
				//rotationAmount.z = 0;
				rotationAmount.z = transform.eulerAngles.z;

				//Used to set the amount of shake (% * startAmount).
				shakePercentage = shakeDuration / initShakeDuration;

				//Set the amount of shake (% * startAmount).
				shakeAmount = initShakeAmount * shakePercentage;

				//Lerp the time, so it is less and tapers off towards the end.
				shakeDuration = Mathf.MoveTowards(shakeDuration, 0, Time.deltaTime);

				if (smooth)
				{
					transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
				}
				else
				{
					//Set the local rotation the be the rotation amount.
					transform.localRotation = Quaternion.Euler(rotationAmount);
				}

				yield return null;
			}

			//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
			//transform.localRotation = Quaternion.identity;
			transform.eulerAngles = initEulerAngles;

			isRunning = false;
		}
	}
}