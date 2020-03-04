using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Layer1 : MonoBehaviour
{
    public int index;
    public List<Transform> UpAccess = new List<Transform>();
    public List<Transform> DownAccess = new List<Transform>();
    private void Start()
    {
        Transform access = transform.Find("Access");
        //UP
        Transform up = access.Find("UP");
        foreach(Transform Upaccess in up){
            UpAccess.Add(Upaccess);
        }

        //DOWN
        Transform down = access.Find("DOWN");
        foreach (Transform Downaccess in down)
        {
            DownAccess.Add(Downaccess);
        }
    }

    public Vector3 GetClosestAccess(int direction, Vector3 pos)
    {
        List<Transform> access;
        if (direction > 0)
            access = UpAccess;
        else
            access = DownAccess;
        access = access.OrderBy(accessTransform => Vector3.Distance(pos, accessTransform.position)).ToList();
        if (access.Count > 0)
            return access[0].position;
        else
            return Vector3.zero;
    }
}
