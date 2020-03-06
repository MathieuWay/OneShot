using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace oneShot
{
    
    public class PatternEditor : Editor
    {
		private void OnSceneGUI()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // do stuff
                Debug.Log(hit.point);
            }
        }
    }
}
