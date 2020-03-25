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
        //private Animation animation;
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
            //animation = GetComponentInChildren<Animation>();
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
            if (!Application.isPlaying || stepsLoaded.Count == 0) return;
            float currentTime = 0f;
            if (UI_Timeline.Instance)
                currentTime = UI_Timeline.Instance.GetCurrentTime();
            //Debug.Log(stepsLoaded[i].targetPos);
            Step current = GetCurrentStep(currentTime);
            switch (current.type)
            {
                case StepType.Idle:
                    if (!current.stepFlag)
                    {
                        /*
                        animation.Stop();
                        animation["idle"].speed = GameTime.Instance.TimeSpeed;
                        animation.CrossFade("idle");
                        */
                        anim.Play("idle");
                        current.stepFlag = true;
                    }
                    anim.speed = GameTime.Instance.TimeSpeed;
                    break;
                case StepType.Move:
                    StepMove stepMove = new StepMove(current);
                    //Debug.Log(current.targetPos);
                    //Debug.Log(stepMove.GetPositionByTime(currentTime));
                    //animation["move"].speed = StepMove.GetMoveFactor(current.moveType) * GameTime.Instance.TimeSpeed;
                    //animation.Play("move");
                    if(!current.stepFlag)
                    {
                        anim.Play(StepMove.GetClipName(current.moveType));
                        current.stepFlag = true;
                    }
                    anim.speed = StepMove.GetMoveFactor(current.moveType) * GameTime.Instance.TimeSpeed;
                    transform.position = stepMove.GetPositionByTime(currentTime);
                    break;
                case StepType.Anim:
                    if (!current.stepFlag)
                    {
                        //animation.Stop();
                        //animation.Play(current.clip.name);
                        anim.Play(current.clip.name);
                        current.stepFlag = true;
                    }
                    anim.speed = GameTime.Instance.TimeSpeed;
                    //animation[current.clip.name].speed = GameTime.Instance.TimeSpeed;
                    break;
                default:
                    break;
            }
        }

        private void ResetPattern()
        {
            enemy.ResetEnemy();
            stepsLoaded = new List<Step>(steps);
            foreach(Step step in stepsLoaded)
            {
                step.stepFlag = false;
            }
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
                        /*if (step.clip)
                        {
                            animation.AddClip(step.clip, step.clip.name);
                            step.duration = step.clip.length;
                        }*/
                        if (step.clip)
                        {
                            step.duration = step.clip.length;
                        }
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
