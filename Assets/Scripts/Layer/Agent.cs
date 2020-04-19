using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
namespace oneShot
{
    public class Agent : MonoBehaviour
    {
        public int layerIndex;
        public oneShot.Layer currentLayer;
        public float speed;
        public bool reach = true;
        public Vector3 target = Vector3.zero;
        private float clock;
        public bool debug;
        public Vector3 initialPosition;
        private oneShot.Enemy enemy;
        // Start is called before the first frame update
        void Awake()
        {
            enemy = GetComponent<oneShot.Enemy>();
            //StartCoroutine("StepLayer");
            initialPosition = transform.position;
        }
        private void Start()
        {
            currentLayer = LayersController.instance.GetLayer(LayersController.instance.GetLayerIndexByHeight(transform.position.y));
        }

        // Update is called once per frame
        void Update()
        {
            if (target != Vector3.zero && !reach && enemy.isAlive)
            {
                if (transform.position.y == target.y)
                {
                    if (transform.position != target && !reach)
                        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime * GameTime.Instance.TimeSpeed);
                    else
                    {
                        reach = true;
                        if (debug)
                            Debug.Log("real time:" + (Time.time - clock));
                    }
                }
                else
                {
                    int direction;
                    if (transform.position.y < target.y)
                        direction = 1;
                    else
                        direction = -1;
                    Transform accessPos = currentLayer.GetClosestAccess(direction, transform.position);
                    if (Vector3.Distance(transform.position, accessPos.position) == 0)
                    {
                        currentLayer = LayersController.instance.GetLayer(currentLayer.index + direction);
                        ChangeLayer();
                    }
                    else
                        transform.position = Vector3.MoveTowards(transform.position, accessPos.position, speed * Time.deltaTime * GameTime.Instance.TimeSpeed);
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
                    target = normalizePos;
                    reach = false;
                    clock = Time.time;
                }
            }
        }

        private void ChangeLayer()
        {
            Vector3 newPos = transform.position;
            newPos.y = currentLayer.transform.position.y;
            transform.position = newPos;
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
            enemy.anim.SetBool("isMoving", false);
            enemy.anim.Rebind();

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
}
#endif
