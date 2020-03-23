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

		[SerializeField] private GameObject tpStartParticle;
		[SerializeField] private GameObject tpFinishParticle;
		private bool launchStartParticle;
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
					launchStartParticle = true;
					Instantiate(tpStartParticle, player.transform.position, tpStartParticle.transform.rotation);
				}

				if (time >= nextStep.time)
				{
					//GameTime.Instance.SetTimeSpeed(0.2f, 1);
					ExecuteNextStep();
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

			OnPlayerTeleport?.Invoke();

			if (tpQueue.Count > 0)
				nextStep = tpQueue.Dequeue();
			else
				isFinished = true;

		}

        public void loadTactics(List<SpawnPoint> tpList)
        {
            tpQueue = new Queue<TacticsStep>();
            foreach (SpawnPoint tp in tpList)
            {
                TacticsStep step = new TacticsStep(tp._Position, tp._Time);
                tpQueue.Enqueue(step);
            }
            nextStep = tpQueue.Dequeue();
        }
    }

}