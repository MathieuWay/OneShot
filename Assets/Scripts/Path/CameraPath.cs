using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace oneShot
{
    public class CameraPath : MonoBehaviour
    {
        public Node CurrentNode;
        public float pourcentage;
        public Node previous;
        public Node next;
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
            previous = CurrentNode.GetNeighbourNodesFromDirection(Direction.Left);
            next = CurrentNode.GetNeighbourNodesFromDirection(Direction.Right);
        }
    }
}
