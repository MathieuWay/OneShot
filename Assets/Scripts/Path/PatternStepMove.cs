using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
    [Serializable]
    public class PatternStepMove : PatternStep
    {
        public Vector3 target;

        public PatternStepMove(float time, Vector3 pos)
        {
            stepType = StepType.Move;
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
