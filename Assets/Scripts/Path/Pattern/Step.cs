using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace oneShot
{
    [System.Serializable]
    public class StepMovePath
    {
        public Vector3 waypoint;
        public float time;

        public StepMovePath(Vector3 vector, float startTime)
        {
            this.waypoint = new Vector3(vector.x, vector.y, vector.z);
            this.time = startTime;
        }
    }

    public enum StepType
    {
        Idle,
        Move,
        Anim

    }

    public enum MoveType
    {
        Walk,
        Run,
        Sprint
    }

    [System.Serializable]
    public class Step
    {
        public StepType type;
        public float startTime;
        public float endTime;
        public float duration;

        //moveData
        public MoveType moveType;
        public Vector3 targetPos;
        [HideInInspector] public List<StepMovePath> stepMovePaths = new List<StepMovePath>();

        //animData
        public AnimationClip clip;
        [HideInInspector] public bool stepFlag;

        public float GetStartTime() { return startTime; }
    }
}
