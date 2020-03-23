using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot.old
{
    public enum StepTypeOld
    {
        Move,
        Anim

    }
    [Serializable]
    public class PatternStepOld
    {
        public StepTypeOld stepType;
        public float startTime;
        /*public virtual void Init() { }
        public virtual void Update() { }
        public virtual bool EndCondition() { return true; }*/
    }
}
