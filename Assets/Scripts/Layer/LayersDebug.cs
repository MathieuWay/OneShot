using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LayersController))]
public class LayersDebug : Editor
{
    protected virtual void OnSceneGUI()
    {
        /*GameObject[] etages = GameObject.FindGameObjectsWithTag("Etage");
        foreach (GameObject etage in etages)
        {
            Layer1 layer = etage.GetComponent<Layer1>();
            Handles.color = Handles.yAxisColor;
            foreach (Transform access in layer.UpAccess)
            {
                Handles.ArrowHandleCap(0, access.position, access.rotation * Quaternion.Euler(-90, 0, 0), 0.4f, EventType.Repaint);
            }


            Handles.color = Handles.xAxisColor;
            foreach (Transform access in layer.DownAccess)
            {
                Handles.ArrowHandleCap(0, access.position, access.rotation * Quaternion.Euler(90, 0, 0), 0.4f, EventType.Repaint);
            }
        }*/
    }
}
