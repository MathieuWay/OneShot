using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Agent : MonoBehaviour
{
    public int layerIndex;
    public Layer1 currentLayer;
    public float speed;
    public bool reach = true;
    public Vector3 target = Vector3.zero;
    private float clock;
    public List<Vector3> path = new List<Vector3>();
    public bool debug;
    private Vector3 initialPosition;
	private oneShot.Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
		enemy = GetComponent<oneShot.Enemy>();
        //StartCoroutine("StepLayer");
        initialPosition = transform.position;
        currentLayer = LayersController.instance.GetLayer(LayersController.instance.GetLayerIndexByHeight(transform.position.y));
    }

    // Update is called once per frame
    void Update()
    {
        if(target != Vector3.zero && !reach && enemy.isAlive)
        {
            if (transform.position.y == target.y)
            {
                if (transform.position != target && !reach)
                    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                else
                {
                    reach = true;
                    if(debug)
                        Debug.Log("real time:"+(Time.time - clock));
                }
            }
            else
            {
                int direction;
                if (transform.position.y < target.y)
                    direction = 1;
                else
                    direction = -1;
                Vector3 accessPos = currentLayer.GetClosestAccess(direction, transform.position);
                if (Vector3.Distance(transform.position, accessPos) == 0){
                    currentLayer = LayersController.instance.GetLayer(currentLayer.index + direction);
                    ChangeLayer();
                }
                else
                    transform.position = Vector3.MoveTowards(transform.position, accessPos, speed * Time.deltaTime);
            }
        }

        if (false)//DEBUG TARGET
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 8;
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                Vector3 normalizePos = new Vector3(hit.point.x, (int)hit.point.y, 0);
                float time = CalculateTime(normalizePos);
                if(time >= 0)
                {
                    target = normalizePos;
                    reach = false;
                    clock = Time.time;
                    if (debug)
                    {
                        Debug.Log(time);
                        StartCoroutine(delay(time));
                    }
                }
                //int targetIndex = LayersController.instance.GetLayerIndexByHeight(target.y);
                //Debug.Log("target:"+targetIndex+"    /   current"+layerIndex);
                //if (targetIndex != layerIndex)
                //    StartCoroutine(StepLayer(targetIndex));
            }
        }
    }

    private void ChangeLayer()
    {
        Vector3 newPos = transform.position;
        newPos.y = currentLayer.transform.position.y;
        transform.position = newPos;
    }

    public float CalculateTime(Vector3 position)
    {
        path.Clear();
        float time = 0;
        Vector3 cursor = transform.position;
        Layer1 layer = currentLayer;
        path.Add(cursor);
        while (cursor.y != position.y)
        {
            Debug.Log(layer.index);
            Debug.Log("currentposY:" + cursor.y + "    targetY:" + position.y);
            int direction;
            if (transform.position.y < position.y)
                direction = 1;
            else
                direction = -1;
            Vector3 access = layer.GetClosestAccess(direction, cursor);
            Debug.Log(access);
            if (access == Vector3.zero)
            {
                path.Clear();
                Debug.LogError("No access Found");
                Debug.Break();
                return -1;
            }
            //Path to access
            time += Vector3.Distance(cursor, access) / speed;
            cursor = access;
            path.Add(access);

            //ChangeLayer
            layer = LayersController.instance.GetLayer(layer.index + direction);
            Vector3 nextPos = cursor;
            nextPos.y = layer.transform.position.y;
            cursor = nextPos;
            path.Add(nextPos);
        }
        time += Vector3.Distance(cursor, position) / speed;
        path.Add(position);
        return time;
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Handles.color = Handles.zAxisColor;
            if (target != Vector3.zero)
            {
                Vector3 offset = target;
                offset.y += 0.5f;
                Handles.ArrowHandleCap(0, offset, Quaternion.Euler(90, 0, 0), 0.4f, EventType.Repaint);
            }

            Gizmos.color = Color.green;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }

    public void SetTarget(Vector3 pos)
    {
        target = pos;
        reach = false;
    }

    public void ResetAgent()
    {
		enemy.isAlive = true;
        target = Vector3.zero;
        transform.position = initialPosition;
        currentLayer = LayersController.instance.GetLayer(LayersController.instance.GetLayerIndexByHeight(transform.position.y));
    }

    IEnumerator delay(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Path ended");
    }
}
