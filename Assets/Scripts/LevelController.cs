using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace oneShot
{
    public enum Phase
    {
        Tactical,
        Combat
    }
    public class LevelController : MonoBehaviour
    {
        private static LevelController instance;
        public static LevelController Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                GameObject inScene = GameObject.Find("LevelController");
                if (inScene)
                {
                    instance = inScene.GetComponent<LevelController>();
                    return instance;
                }
                else
                {
                    Debug.LogError("no LevelController instance in scene");
                    return null;
                }
            }
        }

		public delegate void LevelDelegate();
		public event LevelDelegate OnStartCombatPhase;
		public event LevelDelegate OnPlayerDie;
		public event LevelDelegate OnTimeElapsed;

        public Phase phase;

        private void Awake()
        {
            instance = this;
            Fader.OnFadeIn += FocusOnPlayer;
			UI_Timeline.OnTimeElapsed += delegate { OnTimeElapsed?.Invoke(); };
		}

		private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.Return) || Gamepad.Instance.ButtonDownStart) && phase == Phase.Tactical)
            {
                CameraController.Instance.FocusOnPlayer();
                UI_Timeline.Instance.SetPause(false);
                StartCoroutine(DelayBeforeCombatPhase(CameraController.Instance.focusTime));
            }

			if(Input.GetKeyDown(KeyCode.R))
			{
				ReloadScene(1);
			}
            if (Input.GetKeyDown(KeyCode.T))
            {
                UI_Timeline.Instance.ResetTimeline();
            }
        }


        private void FocusOnPlayer()
        {
            if (phase != Phase.Combat)
                CameraController.Instance.FocusOnPlayer();
        }

        private IEnumerator DelayBeforeCombatPhase(float sec)
        {
			CursorController.Instance.SetState(false);

            yield return new WaitForSeconds(sec);
            TacticsController.Instance.loadTactics(SpawnController.Instance.SpawnPoints);
			phase = Phase.Combat;
			OnStartCombatPhase?.Invoke();
			UI_Timeline.Instance.ResetTimeline();
        }

        public List<Enemy> GetEnemies()
        {
            List<Enemy> enemies = new List<Enemy>();
            foreach (GameObject enemyGameObject in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemies.Add(enemyGameObject.GetComponent<Enemy>());
            }
            return enemies;
        }

		public void ReloadScene(float delay)
		{
			StartCoroutine(ReloadDelay(delay));
		}
		private IEnumerator ReloadDelay(float delay)
		{
			Fader.Instance.FadeOut();
			yield return new WaitForSeconds(delay);
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public void PlayerDie()
		{
			OnPlayerDie?.Invoke();
		}
    }
}

