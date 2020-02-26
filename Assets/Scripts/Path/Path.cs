using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace oneShot
{

    public enum DirectionAxis
    {
        Horizontal,
        Vertical
    }
    public class Path : MonoBehaviour
    {
        public DirectionAxis directionAxis;
        public Node CurrentNode;
        private Node previous;
        private Node next;
        public float pourcentage;
        private void Update()
        {
            if (pourcentage < 0)
            {
                if (previous)
                {
                    SetNode(previous);
                    pourcentage = 1;
                }
                else
                    pourcentage = 0;
            }
            else if (pourcentage > 1)
            {
                if (next)
                {
                    SetNode(next);
                    pourcentage = 0;
                }
                else
                    pourcentage = 1;
            }
        }

        public Vector3 GetPositionAlongPath()
        {
            if (next)
                return Vector3.Lerp(CurrentNode.transform.position, next.transform.position, pourcentage);
            else
                return CurrentNode.transform.position;
        }

        public void SetNode(Node node)
        {
            CurrentNode = node;
            previous = CurrentNode.GetNeighbourNodesFromDirection(GetDirectionFromAxe(directionAxis, -1));
            next = CurrentNode.GetNeighbourNodesFromDirection(GetDirectionFromAxe(directionAxis, 1));
        }

        public void SetDirection(DirectionAxis directionAxis)
        {
            this.directionAxis = directionAxis;
        }

        private Direction GetDirectionFromAxe(DirectionAxis directionAxis, int direction)
        {
            if(directionAxis == DirectionAxis.Horizontal)
            {
                if (direction == 1)
                    return Direction.Right;
                else
                    return Direction.Left;
            }
            else
            {
                if (direction == 1)
                    return Direction.Top;
                else
                    return Direction.Bottom;
            }
        }

        public void AddPourcentage(float pourcentage)
        {
            if(!((!next && this.pourcentage > 0) || (!previous && this.pourcentage < 0)))
                this.pourcentage += pourcentage;
            else
            {
                if (!next)
                    this.pourcentage = 0;
            }
        }
    }
}
