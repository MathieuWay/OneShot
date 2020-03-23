using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace oneShot
{
    [ExecuteInEditMode]
    public class Pattern : MonoBehaviour
    {
        private Animator anim;
        //private Agent agent;
        private Enemy enemy;
        private Vector3 initialPosition;
        //PATTERN
        [ReorderableList]
        public List<Step> steps = new List<Step>();
        private List<Step> stepsLoaded = new List<Step>();
        public Step currentStep = null;

        private void OnEnable()
        {
            CalculatePattern();
        }
        private void OnValidate()
        {
            CalculatePattern();
        }

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            enemy = GetComponent<Enemy>();
            //agent = GetComponent<Agent>();

            CalculatePattern();
            stepsLoaded = new List<Step>(steps);
            UI_Timeline.OnTimelineReset += ResetPattern;
        }
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!Application.isPlaying) return;
            float currentTime = 0f;
            if (UI_Timeline.Instance)
                currentTime = UI_Timeline.Instance.GetCurrentTime();
            //Debug.Log(stepsLoaded[i].targetPos);
            Step current = GetCurrentStep(currentTime);
            switch (current.type)
            {
                case StepType.Idle:
                    break;
                case StepType.Move:
                    StepMove stepMove = new StepMove(current);
                    //Debug.Log(current.targetPos);
                    //Debug.Log(stepMove.GetPositionByTime(currentTime));
                    transform.position = stepMove.GetPositionByTime(currentTime);
                    break;
                case StepType.Anim:
                    break;
                default:
                    break;
            }
        }

        private void ResetPattern()
        {
            enemy.ResetAgent();
            stepsLoaded = new List<Step>(steps);
        }

        private Step GetCurrentStep(float currentTime)
        {
            int i = 0;
            while (i < stepsLoaded.Count)
            {
                if (currentTime <= stepsLoaded[i].GetStartTime())
                    break;
                i++;
            }
            if (i > 0)
                i--;
            //Debug.Log(i);
            return stepsLoaded[i];
        }

        private void CalculatePattern()
        {
            if (!enemy) return;
            float time = 0;
            Vector3 pos = transform.position;
            foreach (Step step in steps)
            {
                step.startTime = time;
                switch (step.type)
                {
                    case StepType.Idle:
                        //StepIdle stepIdle = (StepIdle)step;
                        //time += stepIdle.Load(time);
                        break;
                    case StepType.Move:
                        //StepMove stepmove = (StepMove)step;
                        StepMove stepmove = new StepMove(step);
                        step.duration = stepmove.Load(time, StepMove.GetMoveFactor(step.moveType) * enemy.speed, pos);
                        pos = stepmove.targetPos;
                        break;
                    case StepType.Anim:
                        break;
                    default:
                        break;
                }
                step.endTime = step.startTime + step.duration;
                time += step.duration;
            }
        }
    }
}
