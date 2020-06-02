using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace oneShot
{
    [System.Serializable]
    public struct TacticsStep
    {
        public Vector3 pos;
        public float time;

        public TacticsStep(Vector2 position, float time) : this()
        {
            pos = new Vector3(position.x, position.y, 0);
            this.time = time;
        }
    }
    public class TacticsController : MonoBehaviour
    {
        private static TacticsController instance;
        public static TacticsController Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                GameObject inScene = GameObject.Find("TacticsController");
                if (inScene)
                {
                    instance = inScene.GetComponent<TacticsController>();
                    return instance;
                }
                else
                {
                    Debug.LogError("no TacticsController instance in scene");
                    return null;
                }
            }
        }

		

		public delegate void TacticsDelegate();
		public event TacticsDelegate OnPlayerTeleport;
		public event TacticsDelegate OnBeforePlayerTeleport;

		[Header("Main")]
		[SerializeField] private float timelineDuration = 10;
		[SerializeField] private AnimationCurve fastSpeedCurve = null;
		[SerializeField] private float fastSpeedFactor = 1;
		[SerializeField] private float fastSpeedDelay = 2;
		[SerializeField] private GameObject tpStartParticle = null;
		[SerializeField] private GameObject tpFinishParticle = null;
		private bool launchStartParticle;
		private float newStepTime;
		private float fastSpeedTime;
		private float fastSpeedDuration;

		public float TimelineDuration { get => timelineDuration; }

		public bool isFinished;
        //TIMER
        public float time;
        //STEPS
        public Step[] tpArray;
        private Queue<TacticsStep> tpQueue;
        private TacticsStep nextStep;
        private GameObject player;
        // Start is called before the first frame update
        private void Awake()
        {
            instance = this;
            player = GameObject.FindWithTag("Player");
			isFinished = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (LevelController.Instance.phase == Phase.Combat && !isFinished && !PlayerBehaviour.Instance.IsDead)
            {
				//FX
				if ((time >= nextStep.time - 0.5f && time < nextStep.time) && !launchStartParticle)
				{
					OnBeforePlayerTeleport?.Invoke();
					launchStartParticle = true;
					Instantiate(tpStartParticle, player.transform.position, tpStartParticle.transform.rotation);
					SoundManager.Instance.PlaySound("teleport_01");
				}

				//Fast Speed
				if(time >= newStepTime + fastSpeedDelay)
				{
					GameTime.Instance.SetHardTimeSpeed(1 + fastSpeedCurve.Evaluate(fastSpeedTime / fastSpeedDuration) * fastSpeedFactor);
					fastSpeedTime += Time.deltaTime * GameTime.Instance.TimeSpeed;
				}

				if (time >= nextStep.time)
				{
					ExecuteNextStep();
					GameTime.Instance.SetTimeSpeed(0.1f, 1);
				}
                    
                time += Time.deltaTime * GameTime.Instance.TimeSpeed;
            }
        }

        private void ExecuteNextStep()
        {
            player.transform.position = nextStep.pos;

			//FX
			launchStartParticle = false;
			Instantiate(tpFinishParticle, player.transform.position, tpFinishParticle.transform.rotation);
			SoundManager.Instance.PlaySound("teleport_02");

			OnPlayerTeleport?.Invoke();

			if (tpQueue.Count > 0)
				nextStep = tpQueue.Dequeue();
			else
				isFinished = true;

			//Fast Speed
			SetFastSpeed();

			//Reset time speed
			GameTime.Instance.SetHardTimeSpeed(1);
		}

        public void loadTactics(List<SpawnPoint> tpList)
        {
            tpQueue = new Queue<TacticsStep>();
            foreach (SpawnPoint tp in tpList)
            {
                TacticsStep step = new TacticsStep(tp._Position, tp._Time);
                tpQueue.Enqueue(step);
            }
			if (tpQueue.Count > 0)
				nextStep = tpQueue.Dequeue();
			else
				isFinished = true;
			//Fast Speed
			SetFastSpeed();
		}

		private void SetFastSpeed()
		{
			newStepTime = time;
			fastSpeedTime = 0;
			float durationBetweenStep = nextStep.time - newStepTime;
			if (durationBetweenStep > fastSpeedDelay)
			{
				fastSpeedDuration = nextStep.time - (newStepTime + fastSpeedDelay);
			}
		}
    }
}