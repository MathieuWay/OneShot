﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
    public class EnemiesController : MonoBehaviour
    {
		public static EnemiesController Instance { get; private set; }
		public int EnemyKilledCount { get => enemyCount - currentEnemyCount; }
		public int EnemyLeft { get => currentEnemyCount; }

		public int enemyCount;
        private int currentEnemyCount;

		public delegate void EnemyDelegate();
		public event EnemyDelegate OnAllEnemiesKilled;

        private void Awake()
        {
			if(Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;

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
				OnAllEnemiesKilled?.Invoke();
            }
        }

        private void ResetEnemyCount()
        {
            currentEnemyCount = enemyCount;
        }
    }
}
