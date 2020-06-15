﻿using System.Collections;
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
		public event LevelDelegate OnLaunchCombatPhase;
		public event LevelDelegate OnStartCombatPhase;
		public event LevelDelegate OnPlayerDie;
		public event LevelDelegate OnTimeElapsed;
		public event LevelDelegate OnReloadScene;

        public Phase phase;

		public bool LockCombatPhase { get; set; }
		private bool combatPhaseLaunched = false;


		private void Awake()
        {
            instance = this;
            Fader.OnFadeIn += FocusOnPlayer;
			UI_Timeline.OnTimeElapsed += delegate { OnTimeElapsed?.Invoke(); };
		}

		private void Start()
		{
			EnemiesController.Instance.OnAllEnemiesKilled += Victory;
		}

		private void Update()
        {
            if (!combatPhaseLaunched && !LockCombatPhase && (Input.GetKeyDown(KeyCode.Return) || Gamepad.Instance.ButtonDownY) && phase == Phase.Tactical)
            {
				combatPhaseLaunched = true;
                CameraController.Instance.FocusOnPlayer();
				//UI_Timeline.Instance.SetPause(false);
				UI_Timeline.Instance.BeginCarnage();
				//SoundManager.Instance.PlaySound("start_carnage");
                StartCoroutine(DelayBeforeCombatPhase(CameraController.Instance.focusTime));

				OnLaunchCombatPhase?.Invoke();
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

		private void Victory()
		{
			//Verrouille les combos
			ComboController.Instance.LockCombo = true;
		}

        private void FocusOnPlayer()
        {
            if (phase != Phase.Combat)
                CameraController.Instance.FocusOnPlayer();
        }

        private IEnumerator DelayBeforeCombatPhase(float sec)
        {
			CursorController.Instance.SetState(false);
			UI_Timeline.Instance.ResetTimeline();
			UI_Timeline.Instance.SetPause(true);

			yield return new WaitForSeconds(sec);
			UI_Timeline.Instance.SetPause(false);
			TacticsController.Instance.loadTactics(SpawnController.Instance.SpawnPoints);
			phase = Phase.Combat;
			OnStartCombatPhase?.Invoke();
			//UI_Timeline.Instance.ResetTimeline();
        }

        public List<Enemy> GetEnemies()
        {
            List<Enemy> enemies = new List<Enemy>();
            foreach (GameObject enemyGameObject in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemies.Add(enemyGameObject.GetComponent<Enemy>());
            }
			//Debug.Log("ENEMIES: " + enemies.Count);
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
			OnReloadScene?.Invoke();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public void PlayerDie()
		{
			OnPlayerDie?.Invoke();
		}
    }
}

