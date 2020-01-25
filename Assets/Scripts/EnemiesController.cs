using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    public int enemyCount;

    //public delegate void NoEnemiesLeft();
    //public static event NoEnemiesLeft OnNoEnemiesLeft;

    private void Awake()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Enemy.OnEnemyDead += EnemyDied;
    }
    private void EnemyDied()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            Debug.Log("No enemy left");
            //OnNoEnemiesLeft();
            Fader.Instance.FadeOut();
        }
    }
}
