using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
#endif

public class LayersController : MonoBehaviour
{
    public static LayersController instance;
    public List<GameObject> layers;
    private void Awake()
    {
        instance = this;
        layers = GameObject.FindGameObjectsWithTag("Etage").OrderBy(layer => layer.transform.position.y).ToList();
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].GetComponent<Layer1>().index = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetLayerPositionByIndex(int index)
    {
        if(index < layers.Count)
            return layers[index].transform.position.y;
        else
            return 0;
    }

    public int GetLayerIndexByHeight(float height)
    {
        int i = 0;
        while(i < layers.Count)
        {
            if (height == layers[i].transform.position.y)
                return i;
            else if (height <= layers[i].transform.position.y)
                return i - 1;
            else
                i++;
        }
        return 0;
    }

    public Layer1 GetLayer(int index)
    {
        return layers[index].GetComponent<Layer1>();
    }

#if UNITY_EDITOR
	private void OnDrawGizmos()
    {
        foreach (GameObject etage in layers)
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
        }
    }
#endif
}
