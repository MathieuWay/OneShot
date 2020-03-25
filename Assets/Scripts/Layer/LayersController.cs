using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LayersController : MonoBehaviour
{
    public static LayersController instance;
    public List<GameObject> layers = new List<GameObject>();

    private void OnEnable()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Debug.Log("init layer controller");
        instance = this;
        layers = GameObject.FindGameObjectsWithTag("Etage").OrderBy(layer => layer.transform.position.y).ToList();
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].GetComponent<oneShot.Layer>().index = i;
            layers[i].GetComponent<oneShot.Layer>().LoadAccess();
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

    public oneShot.Layer GetLayer(int index)
    {
        return layers[index].GetComponent<oneShot.Layer>();
    }
}
