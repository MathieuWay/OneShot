using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        public Phase phase;

        private void Awake()
        {
            instance = this;
            Fader.OnFadeIn += FocusOnPlayer;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && phase == Phase.Tactical)
            {
                CameraController.Instance.FocusOnPlayer();
                UI_Timeline.Instance.SetPause(false);
                StartCoroutine(DelayBeforeCombatPhase(CameraController.Instance.focusTime));
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
            phase = Phase.Combat;
            TacticsController.Instance.loadTactics(SpawnController.Instance.SpawnPoints);
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
    }
}

