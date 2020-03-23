using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

namespace oneShot.old
{
    public class PatternOld : MonoBehaviour
    {
        /*
        [ReorderableList]
        public List<PatternStepMoveOld> patternSteps = new List<PatternStepMoveOld>();
        private List<PatternStepMoveOld> patternStepsLoaded = new List<PatternStepMoveOld>();

        //private PatternStepMove currentStep = null;
        private Animator anim;
        private Agent agent;
        private Vector3 initialPosition;
        //private List<Vector3> path = new List<Vector3>();
        public List<PatternStepMoveOld> pathWithTime = new List<PatternStepMoveOld>();

        public bool RecordPattern;
        public bool debug;

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            agent = GetComponent<Agent>();

            patternStepsLoaded = new List<PatternStepMoveOld>(patternSteps);
            UI_Timeline.OnTimelineReset += ResetPattern;
            initialPosition = transform.position;
        }

        private void Start()
        {
            CalculatePattern();
        }

        public void Update()
        {
            if (patternStepsLoaded.Count > 0)
            {
                if (patternStepsLoaded[0].startTime <= UI_Timeline.Instance.GetCurrentTime())
                    GetNextStep();
            }

            if (RecordPattern && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layerMask = 1 << 8;
                if (Physics.Raycast(ray, out hit, layerMask))
                {
                    Vector3 normalizePos = new Vector3(hit.point.x, (int)hit.point.y, 0);
                    //Debug.Log(normalizePos);
                    patternSteps.Add(new PatternStepMoveOld(UI_Timeline.Instance.GetCurrentTime(), normalizePos));
                    CalculatePattern();
                    //Transform objectHit = hit.transform;
                    // Do something with the object that was hit by the raycast.
                }
            }
        }

        private void GetNextStep()
        {
            if (patternStepsLoaded.Count > 0)
            {
                //currentStep = patternSteps.Dequeue();
                PatternStepMoveOld step = patternStepsLoaded[0];
                patternStepsLoaded.RemoveAt(0);
                switch (step.stepType)
                {
                    case StepTypeOld.Move:
                        agent.SetTarget(step.target);
                        //moveStep.InitMove(GetComponent<Path>());
                        break;
                }
            }
        }

        private void ResetPattern()
        {
            agent.ResetAgent();
            patternStepsLoaded = new List<PatternStepMoveOld>(patternSteps);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (PatternStepMoveOld moveStep in patternSteps)
            {
                Gizmos.DrawSphere(moveStep.target, 0.1f);
            }

            if (debug)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < pathWithTime.Count - 1; i++)
                {
                    Gizmos.DrawLine(pathWithTime[i].pos, pathWithTime[i + 1].pos);
                }

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(GetPositionWithTime(), 0.2f);
            }
        }

        private void OnValidate()
        {
            if (EditorApplication.isPlaying && agent)
            {
                CalculatePattern();
            }
        }

        private void CalculatePattern()
        {
            //path.Clear();
            pathWithTime.Clear();
            float time = 0;
            for (int i = 0; i < patternSteps.Count; i++)
            {
                Vector3 initialPos = agent.initialPosition;
                if (i > 0)
                {
                    initialPos = patternSteps[i - 1].target;
                }
                //Debug.Log("initial:" + initialPos + "      /target:" + patternSteps[i].target);
                patternSteps[i].duration = CalculateTime(initialPos, patternSteps[i].target, agent.speed, ref time);
            }
        }

        public float CalculateTime(Vector3 initialPos, Vector3 position, float speed, ref float time)
        {
            float duration = 0f;
            Vector3 cursor = initialPos;
            oneShot.Layer layer = LayersController.instance.GetLayer(LayersController.instance.GetLayerIndexByHeight(cursor.y));
            //path.Add(cursor);
            pathWithTime.Add(new Step(new Vector2(cursor.x, cursor.y), time));
            while (cursor.y != position.y)
            {
                int direction;
                if (cursor.y < position.y)
                    direction = 1;
                else
                    direction = -1;
                //Debug.Log("position:" + cursor + "  /direction:" + direction);
                Vector3 access = layer.GetClosestAccess(direction, cursor);
                if (access == Vector3.zero)
                {
                    //path.Clear();
                    pathWithTime.Clear();
                    Debug.LogError("No access Found");
                    Debug.Break();
                    return -1;
                }
                //Path to access
                duration += Vector3.Distance(cursor, access) / speed;
                cursor = access;
                //path.Add(access);
                pathWithTime.Add(new Step(new Vector2(access.x, access.y), time + duration));

                //ChangeLayer
                layer = LayersController.instance.GetLayer(layer.index + direction);
                Vector3 nextPos = cursor;
                nextPos.y = layer.transform.position.y;
                cursor = nextPos;
                //path.Add(nextPos);
                pathWithTime.Add(new Step(new Vector2(nextPos.x, nextPos.y), time+ duration));
            }
            duration += Vector3.Distance(cursor, position) / speed;
            time += duration;
            //path.Add(position);
            pathWithTime.Add(new Step(new Vector2(position.x, position.y), time));
            return duration;
        }

        public Vector3 GetPositionWithTime()
        {
            int i = 0;
            float currentTime = UI_Timeline.Instance.GetCurrentTime();
            while (i < pathWithTime.Count)
            {
                if (pathWithTime[i].time > currentTime)
                {
                    if (i > 0)
                        i--;
                    else
                        i = 0;
                    break;
                }
                i++;
            }
            if (i == pathWithTime.Count)
                i-=2;
            //Debug.Log(i);
            float currentPathTime = currentTime - pathWithTime[i].time;
            float pathDuration = pathWithTime[i + 1].time - pathWithTime[i].time;
            float normalize = currentPathTime / pathDuration;
            Vector3 pos = Vector3.Lerp(pathWithTime[i].pos, pathWithTime[i + 1].pos, currentPathTime / pathDuration);
            return pos;
        }*/
    }
}
