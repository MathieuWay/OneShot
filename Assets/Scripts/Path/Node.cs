using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace oneShot
{
    public enum Direction
    {
        Top,
        Right,
        Bottom,
        Left,
        Front,
        Back
    }

    [Serializable]
    public class NeighbourNode
    {
        public Node node;
        public Direction direction;
    }

    public class Node : MonoBehaviour
    {
        public List<NeighbourNode> neighbourNodes = new List<NeighbourNode>();
        public Node GetNeighbourNodesFromDirection(Direction direction)
        {
            NeighbourNode neighbour = neighbourNodes.Find(node => node.direction == direction);
            if (neighbour != null)
                return neighbour.node;
            else
                return null;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.2f);
            foreach(NeighbourNode neighbourNode in neighbourNodes)
            {
                Debug.DrawLine(transform.position, neighbourNode.node.transform.position, Color.green);
            }
        }
    }
}
