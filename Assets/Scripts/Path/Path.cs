using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace oneShot
{

    public enum DirectionAxis
    {
        Horizontal,
        Vertical,
        Depth
    }
    public class Path : MonoBehaviour
    {
        public DirectionAxis directionAxis;
        public Node CurrentNode;
        public Node previous;
        public Node next;
        public float pourcentage;
        public float pathSpeed;

        private void Awake()
        {
            if(CurrentNode)
                SetNode(CurrentNode);
        }

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
                if (next && next.GetNeighbourNodesFromDirection(GetDirectionFromAxe(directionAxis, 1)))
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
            if (!next && previous)
            {
                Node temp = CurrentNode;
                CurrentNode = previous;
                next = temp;
                previous = null;
            }
        }

        public void SetDirection(DirectionAxis directionAxis)
        {
            this.directionAxis = directionAxis;
        }

        public void InitPath(Vector3 initialPos)
        {
            if(next)
                pourcentage = Vector3.Distance(CurrentNode.transform.position, initialPos) / Vector3.Distance(CurrentNode.transform.position, next.transform.position);
            else if(previous)
                pourcentage = Vector3.Distance(CurrentNode.transform.position, initialPos) / Vector3.Distance(CurrentNode.transform.position, next.transform.position);
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
            else if(directionAxis == DirectionAxis.Vertical)
            {
                if (direction == 1)
                    return Direction.Top;
                else
                    return Direction.Bottom;
            }
            else
            {
                if (direction == 1)
                    return Direction.Back;
                else
                    return Direction.Front;
            }
        }

        public void AddPourcentage(float pourcentage)
        {
            if(!((!next && this.pourcentage > 0) || (!previous && this.pourcentage < 0)))
            {
                this.pourcentage += pourcentage;
            }
            else
            {
                if (!next)
                    this.pourcentage = 0;
            }
        }

        public void AddProgression(float direction)
        {
            if (next)
            {
                if (!((!next && this.pourcentage > 0) || (!previous && this.pourcentage < 0)))
                {
                    float speedRatio = pathSpeed / Vector3.Distance(CurrentNode.transform.position, next.transform.position);
                    if (direction > 0)
                        this.pourcentage += speedRatio;
                    else if (direction < 0)
                        this.pourcentage -= speedRatio;
                }
                else
                {
                    if (!next)
                        this.pourcentage = 0;
                }
            }
        }
    }
}
