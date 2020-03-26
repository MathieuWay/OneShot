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
        private Enemy enemy;
        private Vector3 initialPosition;

        //PATTERN
        [ReorderableList]
        public List<Step> steps = new List<Step>();
        private List<Step> stepsLoaded = new List<Step>();

        //DEBUG
        public Step currentStep = null;

        private void OnEnable()
        {
			if(LayersController.instance)
				CalculatePattern();
		}

        private void OnValidate()
        {
			if (LayersController.instance)
				CalculatePattern();
		}

        private void Awake()
        {
            initialPosition = transform.position;
            anim = GetComponentInChildren<Animator>();
            enemy = GetComponent<Enemy>();

            CalculatePattern();
            stepsLoaded = new List<Step>(steps);
            UI_Timeline.OnTimelineReset += ResetPattern;
        }
        
        void Update()
        {
			if (!enemy.isAlive) return;

			if (!Application.isPlaying || stepsLoaded.Count == 0) return;
            float currentTime = 0f;
            if (UI_Timeline.Instance)
                currentTime = UI_Timeline.Instance.GetCurrentTime();
            Step current = GetCurrentStep(currentTime);

            switch (current.type)
            {
                case StepType.Idle:
                    if (!current.stepFlag)
                    {
                        anim.Play("idle");
                        current.stepFlag = true;
                    }
                    anim.speed = GameTime.Instance.TimeSpeed;
                    break;
                case StepType.Move:
                    if(!current.stepFlag)
                    {
                        anim.Play(StepMove.GetClipName(current.moveType));
                        current.stepFlag = true;
                    }
                    anim.speed = StepMove.GetMoveFactor(current.moveType) * GameTime.Instance.TimeSpeed;
                    transform.position = StepMove.GetPositionByTime(current.stepMovePaths, currentTime);
                    break;
                case StepType.Anim:
                    if (!current.stepFlag)
                    {
                        anim.Play(current.clip.name);
                        current.stepFlag = true;
                    }
                    anim.speed = GameTime.Instance.TimeSpeed;
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
            currentStep = stepsLoaded[i];
            return stepsLoaded[i];
        }

        private void CalculatePattern()
        {
            if (!enemy) return;
            float time = 0;
            Vector3 pos = initialPosition;
            foreach (Step step in steps)
            {
                step.startTime = time;
                switch (step.type)
                {
                    case StepType.Idle:
                        break;
                    case StepType.Move:
                        step.stepMovePaths.Clear();
                        step.targetPos.y = LayersController.instance.GetLayer(LayersController.instance.GetLayerIndexByHeight(step.targetPos.y)).transform.position.y;
                        step.duration = StepMove.CalculateTime(step, pos, step.targetPos, StepMove.GetMoveFactor(step.moveType) * enemy.speed, time);
                        pos = step.targetPos;
                        break;
                    case StepType.Anim:
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
