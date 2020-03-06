using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
    public class EnemiesController : MonoBehaviour
    {
        public int enemyCount;
        private int currentEnemyCount;

		public delegate void EnemyDelegate();
		public static event EnemyDelegate OnAllEnemiesKilled;
        //public delegate void NoEnemiesLeft();
        //public static event NoEnemiesLeft OnNoEnemiesLeft;

        private void Awake()
        {
            enemyCount = LevelController.Instance.GetEnemies().Count;
            currentEnemyCount = enemyCount;
            Enemy.OnEnemyDead += EnemyDied;
            UI_Timeline.OnTimelineReset += ResetEnemyCount;
        }
        private void EnemyDied()
        {
            currentEnemyCount--;
            if (currentEnemyCount <= 0)
            {
                Debug.Log("No enemy left");
                //OnNoEnemiesLeft();
                

				OnAllEnemiesKilled?.Invoke();
            }
        }

        private void ResetEnemyCount()
        {
            currentEnemyCount = enemyCount;
        }
    }
}
