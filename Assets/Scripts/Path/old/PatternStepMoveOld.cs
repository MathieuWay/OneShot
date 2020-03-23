using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot.old
{
    [Serializable]
    public class PatternStepMoveOld : PatternStepOld
    {
        public Vector3 target;
        public float duration;

        public PatternStepMoveOld(float time, Vector3 pos)
        {
            stepType = StepTypeOld.Move;
            startTime = time;
            target = pos;
        }
        /*public PatternStepMove(Node GoTo)
        {
            this.GoToNode = GoTo;
        }

        public void InitMove(Path path)
        {
            currentPath = path;
            currentPath.SetDirection(directionAxis);
            currentPath.SetNode(path.CurrentNode);
            if (currentPath.previous == GoToNode)
                direction = -1;
            else
                direction = 1;
        }
        public override void Update()
        {
            currentPath.AddProgression(direction);
        }
        public override bool EndCondition()
        {
            if (
                (currentPath.CurrentNode == GoToNode && currentPath.pourcentage == 0) ||
                (currentPath.previous == GoToNode && currentPath.pourcentage == 0) ||
                (currentPath.next == GoToNode && currentPath.pourcentage == 1)
            )
                return true;
            else
                return false;
        }*/
    }
}
