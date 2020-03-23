using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot.old
{
    public enum StepType
    {
        Move,
        Anim

    }
    [Serializable]
    public class PatternStep
    {
        public StepType stepType;
        private float startTime;
        private float endTime;
        public virtual void Init() { }
        public virtual void CalculateTime(float startTime)
        {

        }
        /*public virtual void Update() { }
        public virtual bool EndCondition() { return true; }*/
    }
}
