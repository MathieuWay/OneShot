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
        public StepType stepType;
        public float startTime;
        /*public virtual void Init() { }
        public virtual void Update() { }
        public virtual bool EndCondition() { return true; }*/
    }
}
