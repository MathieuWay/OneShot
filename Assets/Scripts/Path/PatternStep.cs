using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
    public enum StepType
    {
        Move,
        Anim

    }
    [Serializable]
    public class PatternStep
    {
        public float startTime;
        public StepType stepType;
        public virtual void Init() { }
        public virtual void Update() { }
        public virtual bool EndCondition() { return true; }
    }
}
