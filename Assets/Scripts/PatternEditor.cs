using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace oneShot
{
    public class PatternEditor : MonoBehaviour
    {
        public List<Enemy> enemies;
        private void Awake()
        {
            enemies = LevelController.Instance.GetEnemies();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                float ppp = EditorGUIUtility.pixelsPerPoint;
                mousePos.y = Camera.main.pixelHeight - mousePos.y * ppp;
                mousePos.x *= ppp;

                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.point);
                    //PatternStepMove newStep = new PatternStepMove();
                    //enemies[0].GetComponent<Pattern>().patternSteps.Add();
                }
            }
        }
    }
}
